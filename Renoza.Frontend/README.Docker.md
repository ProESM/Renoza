# Renoza Frontend - Docker Setup

Данный документ описывает процесс запуска Frontend приложения в Docker.

## Требования

- Docker 20.10+
- Docker Compose 1.29+ (опционально)

## Структура файлов

- `Dockerfile` - многоэтапная сборка Angular приложения с nginx
- `nginx.conf` - конфигурация nginx для SPA
- `.dockerignore` - исключения для Docker сборки
- `docker-compose.example.yml` - пример docker-compose файла

## Сборка Docker образа

### Простая сборка

```bash
docker build -t renoza-frontend:latest .
```

### Сборка с тегом версии

```bash
docker build -t renoza-frontend:1.0.0 .
```

## Запуск контейнера

### Запуск с использованием docker run

```bash
docker run -d \
  --name renoza-frontend \
  -p 4200:80 \
  renoza-frontend:latest
```

### Запуск с переменными окружения

```bash
docker run -d \
  --name renoza-frontend \
  -p 4200:80 \
  -e NODE_ENV=production \
  renoza-frontend:latest
```

### Запуск с использованием docker-compose

```bash
# Скопируйте пример файла
cp docker-compose.example.yml docker-compose.yml

# Запустите контейнер
docker-compose up -d
```

## Конфигурация API URL

По умолчанию приложение использует API URL из `environment.prod.ts`:
- Локально: `http://localhost:8080/api`

Для интеграции с Backend в Docker:

### Вариант 1: Создать общую Docker сеть

```bash
# Предположим, что Backend уже запущен в сети renoza-network
docker network create renoza-network

# Запустите Frontend в той же сети
docker run -d \
  --name renoza-frontend \
  --network renoza-network \
  -p 4200:80 \
  renoza-frontend:latest
```

### Вариант 2: Использовать docker-compose с Backend

Создайте общий `docker-compose.yml` в корне проекта:

```yaml
version: '3.8'

services:
  backend:
    build:
      context: ./Renoza.Backend
      dockerfile: Dockerfile
    ports:
      - "8080:8080"
    networks:
      - renoza-network
    environment:
      - ASPNETCORE_ENVIRONMENT=Production

  frontend:
    build:
      context: ./Renoza.Frontend
      dockerfile: Dockerfile
    ports:
      - "4200:80"
    depends_on:
      - backend
    networks:
      - renoza-network

networks:
  renoza-network:
    driver: bridge
```

Запуск:
```bash
docker-compose up -d
```

## Проверка работы

После запуска контейнера:

1. Откройте браузер по адресу: http://localhost:4200
2. Проверьте health endpoint: http://localhost:4200/health

## Логи контейнера

```bash
# Просмотр логов
docker logs renoza-frontend

# Следить за логами в реальном времени
docker logs -f renoza-frontend
```

## Остановка и удаление

```bash
# Остановить контейнер
docker stop renoza-frontend

# Удалить контейнер
docker rm renoza-frontend

# Удалить образ
docker rmi renoza-frontend:latest
```

## Особенности конфигурации

### Nginx

- Порт: 80 (внутри контейнера)
- Gzip сжатие включено
- Кэширование статических ресурсов: 1 год
- HTML файлы не кэшируются
- SPA routing: все запросы перенаправляются на index.html
- Security headers включены

### Multi-stage build

Dockerfile использует двухэтапную сборку:

1. **Build stage**: Сборка Angular приложения (Node.js 18)
2. **Production stage**: Обслуживание через nginx:alpine (минимальный размер образа)

Это обеспечивает:
- Маленький размер финального образа (~50MB)
- Отсутствие dev зависимостей в production
- Оптимизированную сборку

## Troubleshooting

### Проблема: Контейнер не запускается

Проверьте логи:
```bash
docker logs renoza-frontend
```

### Проблема: Приложение не подключается к API

1. Убедитесь, что Backend запущен и доступен
2. Проверьте конфигурацию сети Docker
3. Проверьте `environment.prod.ts` - правильный ли URL API

### Проблема: 404 ошибка при переходе по роутам

Это решается конфигурацией nginx `try_files $uri $uri/ /index.html;` в `nginx.conf`

## Production Deployment

Для production рекомендуется:

1. Использовать конкретные версии образов (не latest)
2. Настроить reverse proxy (nginx, Traefik) с SSL/TLS
3. Настроить мониторинг и логирование
4. Использовать секреты для чувствительных данных
5. Настроить health checks

Пример с health check:

```yaml
services:
  frontend:
    build: .
    healthcheck:
      test: ["CMD", "wget", "--quiet", "--tries=1", "--spider", "http://localhost/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
```
