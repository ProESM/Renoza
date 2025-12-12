# OFD API Resilience Policies - Документация

## Обзор

Реализованы политики устойчивости (Resilience) для вызовов к внешнему API OFD.ru с использованием библиотеки **Polly v8** (современный ResiliencePipeline API).

## Архитектура политик

Применяется **ResiliencePipeline** из 3 уровней защиты:

```
HttpClient → Retry → Circuit Breaker → Timeout → OFD API
```

**Изменения в Polly v8:**
- ✅ Использован современный `ResiliencePipeline` вместо устаревшего `IAsyncPolicy`
- ✅ Убран deprecated пакет `Polly.Extensions.Http`
- ✅ Добавлен **Jitter** в Retry для предотвращения thundering herd эффекта
- ❌ Bulkhead временно убран (можно добавить через `AddConcurrencyLimiter` при необходимости)

### 1. **Timeout Policy**
Жёсткий таймаут на HTTP запрос (внутренний слой, выполняется первым).

**Параметры:**
- `TimeoutSeconds`: 10 (максимальное время ожидания)

**Защита:**
- Предотвращает зависание на неотвечающем API
- Прерывает запрос принудительно после timeout

**Логирование:**
```
⏱️ OFD API Timeout после 10s
```

### 2. **Circuit Breaker Policy**
Размыкает цепь при множественных сбоях, давая время OFD API на восстановление.

**Параметры:**
- `CircuitBreakerFailureThreshold`: 5 (ошибок подряд для размыкания)
- `CircuitBreakerDurationSeconds`: 30 (длительность Open state)

**Состояния:**
- **Closed** (закрыто): Нормальная работа, запросы проходят
- **Open** (открыто): Цепь разомкнута, запросы отклоняются без вызова API
- **Half-Open** (полуоткрыто): Тестовый запрос для проверки восстановления

**Логирование:**
```
⚠️ OFD API Circuit Breaker ОТКРЫТ на 30s. Превышен порог 5 последовательных ошибок.
🔄 OFD API Circuit Breaker в состоянии HALF-OPEN. Тестируем восстановление...
✅ OFD API Circuit Breaker ЗАКРЫТ. Сервис восстановлен.
```

### 3. **Retry Policy**
Автоматически повторяет запрос при временных сбоях с экспоненциальной задержкой + jitter (внешний слой, выполняется последним).

**Параметры:**
- `RetryCount`: 3 (максимум попыток)
- `RetryDelaySeconds`: 2 (базовая задержка)
- `BackoffType`: Exponential
- `UseJitter`: true (**NEW в Polly v8** - предотвращает thundering herd)

**Задержки:** ~2s → ~4s → ~8s (экспоненциальный рост с небольшим случайным отклонением)

**Условия retry:**
- ✅ 5xx (серверные ошибки)
- ✅ 408 Request Timeout
- ✅ Сетевые ошибки (HttpRequestException)
- ✅ Timeout (TimeoutRejectedException)
- ❌ 4xx (клиентские ошибки) - НЕ повторяется
- ❌ 429 Too Many Requests - НЕ повторяется (защита от превышения лимита)

**Логирование:**
```
OFD API Retry 2/3 после 4.2s. StatusCode: 503, Ошибка: Service Unavailable
```

## Конфигурация

### appsettings.json

```json
{
  "OfdApiResilience": {
    "RetryCount": 3,
    "RetryDelaySeconds": 2,
    "CircuitBreakerFailureThreshold": 5,
    "CircuitBreakerDurationSeconds": 30,
    "TimeoutSeconds": 10
  }
}
```

**Примечание:** Параметры `MaxParallelRequests` и `MaxQueuedRequests` сохранены в Options классе на будущее, но временно не используются (Bulkhead убран из pipeline в Polly v8).

### Рекомендуемые значения для разных окружений

**Development:**
```json
{
  "OfdApiResilience": {
    "RetryCount": 2,
    "RetryDelaySeconds": 1,
    "CircuitBreakerFailureThreshold": 3,
    "CircuitBreakerDurationSeconds": 15,
    "TimeoutSeconds": 5
  }
}
```

**Production:**
```json
{
  "OfdApiResilience": {
    "RetryCount": 3,
    "RetryDelaySeconds": 2,
    "CircuitBreakerFailureThreshold": 5,
    "CircuitBreakerDurationSeconds": 30,
    "TimeoutSeconds": 10
  }
}
```

## Мониторинг

### Ключевые логи для мониторинга

1. **Circuit Breaker открыт** → Критический алерт (OFD API недоступен)
2. **Множественные Retry** → Предупреждение (проблемы с сетью/OFD API)
3. **Timeout** → Предупреждение (медленные ответы OFD API)

### Метрики для Prometheus/Grafana (опционально)

- `ofd_api_circuit_breaker_state` (gauge: 0=Closed, 1=Open, 2=HalfOpen)
- `ofd_api_retry_count` (counter)
- `ofd_api_timeout_count` (counter)
- `ofd_api_request_duration_seconds` (histogram)

## Обработка ошибок в Consumer

### CashReceiptRecognitionConsumer

При сбое OFD API:
1. Retry Policy автоматически повторит запрос 3 раза
2. Если все попытки провалились → Consumer обновит статус Job:
   - `CashReceiptJobStatus.RecognitionFailed`
   - `StatusComment`: "Ошибка от OFD API: {детали}"

3. Если Circuit Breaker открыт → запрос отклоняется сразу:
   - Исключение: `BrokenCircuitException`
   - Consumer логирует ошибку и обновляет статус Job

### Пример обработки BrokenCircuitException

```csharp
try
{
    var ofdResponse = await _ofdApiService.GetReceiptAsync(ofdRequest);
}
catch (BrokenCircuitException ex)
{
    _logger.LogError("Circuit Breaker открыт. OFD API временно недоступен.");

    await _cashReceiptJobService.UpdateJobStatusAsync(
        message.JobId,
        CashReceiptJobStatus.RecognitionFailed,
        "OFD API временно недоступен. Повторите попытку позже.");
}
```

## Тестирование

### Ручное тестирование Circuit Breaker

1. **Симуляция сбоя OFD API:**
   - Остановить OFD API mock/сервис
   - Отправить 5 запросов подряд
   - Убедиться, что Circuit Breaker открылся

2. **Проверка Half-Open → Closed:**
   - Подождать 30 секунд
   - Запустить OFD API mock
   - Отправить запрос
   - Убедиться, что Circuit Breaker закрылся

3. **Проверка Retry:**
   - Настроить OFD API mock на возврат 503 (2 раза), затем 200 OK
   - Отправить запрос
   - Убедиться, что запрос успешен после 2 retry

### Unit тесты

Проверить политики можно с помощью тестов:

```csharp
[Fact]
public async Task CircuitBreaker_Opens_After_Threshold_Failures()
{
    // Arrange: настроить mock HttpClient на возврат 500
    // Act: отправить 5 запросов
    // Assert: 6-й запрос должен выбросить BrokenCircuitException
}

[Fact]
public async Task Retry_Succeeds_On_Third_Attempt()
{
    // Arrange: настроить mock на 503 → 503 → 200
    // Act: вызвать GetReceiptAsync
    // Assert: запрос успешен, сделано 3 попытки
}
```

## Отключение политик (для тестирования)

Если нужно временно отключить политики:

```json
{
  "OfdApiResilience": {
    "RetryCount": 0,
    "CircuitBreakerFailureThreshold": 999999,
    "TimeoutSeconds": 300
  }
}
```

## Troubleshooting

### Проблема: Слишком часто открывается Circuit Breaker

**Решение:** Увеличить `CircuitBreakerFailureThreshold` (например, с 5 до 10)

### Проблема: Таймауты при медленных ответах OFD API

**Решение:** Увеличить `TimeoutSeconds` (например, с 10 до 15)

### Проблема: Нужно ограничить параллельные запросы

**Решение:** Добавить Bulkhead в pipeline через `.AddConcurrencyLimiter()` (см. Polly v8 документацию)

### Проблема: Слишком долгая обработка при сбоях

**Решение:** Уменьшить `RetryCount` (например, с 3 до 2)

## Дополнительная информация

- [Polly Documentation](https://github.com/App-vNext/Polly)
- [Circuit Breaker Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker)
- [Retry Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/retry)
