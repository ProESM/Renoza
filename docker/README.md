# Renoza - Docker Deployment

Данная директория содержит конфигурацию для запуска всего стека приложения Renoza в Docker.

## Архитектура

Приложение состоит из следующих сервисов:

1. **PostgreSQL** - База данных (порт 5432)
2. **Backend** - ASP.NET Core API (порт 8080)
3. **Frontend** - Angular SPA с nginx (порт 4200)

Все сервисы работают в единой Docker сети `renoza-network`.

## Требования

- Docker 20.10+
- Docker Compose 1.29+
- Минимум 2GB свободной оперативной памяти
- Минимум 5GB свободного места на диске

## Быстрый старт

### 1. Подготовка окружения

```bash
# Перейдите в директорию docker
cd docker

# Скопируйте файл с переменными окружения
cp .env.example .env

# Отредактируйте .env файл при необходимости
# nano .env  # или используйте любой текстовый редактор
```

### 2. Запуск всех сервисов

```bash
# Запустить все сервисы
docker-compose up -d

# Просмотреть логи
docker-compose logs -f

# Просмотреть логи конкретного сервиса
docker-compose logs -f backend
docker-compose logs -f frontend
```

### 3. Проверка работоспособности

После запуска подождите 30-60 секунд, чтобы все сервисы успели стартовать.

```bash
# Проверить статус сервисов
docker-compose ps

# Проверить health checks
docker-compose ps | grep healthy
```

Приложение доступно по адресам:
- Frontend: http://localhost:4200
- Backend API: http://localhost:8080
- PostgreSQL: localhost:5432

## Конфигурация

### Переменные окружения (.env)

| Переменная | Описание | Значение по умолчанию |
|-----------|----------|---------------------|
| `POSTGRES_DB` | Имя базы данных | renoza |
| `POSTGRES_USER` | Пользователь PostgreSQL | postgres |
| `POSTGRES_PASSWORD` | Пароль PostgreSQL | postgres |
| `POSTGRES_PORT` | Порт PostgreSQL (хост) | 5432 |
| `ASPNETCORE_ENVIRONMENT` | Окружение ASP.NET Core | Production |
| `BACKEND_PORT` | Порт Backend API (хост) | 8080 |
| `FRONTEND_PORT` | Порт Frontend (хост) | 4200 |
| `TZ` | Часовой пояс | Europe/Moscow |

### Изменение портов

Если стандартные порты уже заняты, измените их в `.env`:

```env
POSTGRES_PORT=5433
BACKEND_PORT=8081
FRONTEND_PORT=4201
```

## Управление сервисами

### Запуск и остановка

```bash
# Запустить все сервисы
docker-compose up -d

# Запустить конкретный сервис
docker-compose up -d postgres

# Остановить все сервисы
docker-compose down

# Остановить и удалить volumes (БД будет очищена!)
docker-compose down -v

# Перезапустить сервис
docker-compose restart backend
```

### Сборка образов

```bash
# Пересобрать все образы
docker-compose build

# Пересобрать без кэша
docker-compose build --no-cache

# Пересобрать конкретный сервис
docker-compose build backend
```

### Просмотр логов

```bash
# Все логи
docker-compose logs -f

# Логи конкретного сервиса
docker-compose logs -f backend

# Последние 100 строк
docker-compose logs --tail=100 backend

# Логи с временными метками
docker-compose logs -f -t backend
```

### Выполнение команд внутри контейнеров

```bash
# Подключиться к PostgreSQL
docker-compose exec postgres psql -U postgres -d renoza

# Открыть bash в контейнере backend
docker-compose exec backend bash

# Открыть sh в контейнере frontend (alpine linux)
docker-compose exec frontend sh
```

## Миграции базы данных

Если у вас есть проект миграций `Renoza.DbMigration`:

```bash
# Перейдите в директорию миграций
cd ../Renoza.DbMigration

# Запустите миграции с подключением к Docker PostgreSQL
dotnet run --configuration Release
```

Или можно добавить сервис миграции в `docker-compose.yml`:

```yaml
  migration:
    build:
      context: ../Renoza.DbMigration
      dockerfile: Dockerfile
    environment:
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=renoza;Username=postgres;Password=postgres
    depends_on:
      postgres:
        condition: service_healthy
    networks:
      - renoza-network
    restart: "no"
```

## Резервное копирование и восстановление

### Создание резервной копии БД

```bash
# Создать backup
docker-compose exec postgres pg_dump -U postgres renoza > backup_$(date +%Y%m%d_%H%M%S).sql

# Или через docker cp
docker-compose exec postgres pg_dump -U postgres renoza > /tmp/backup.sql
```

### Восстановление из резервной копии

```bash
# Восстановить из backup
docker-compose exec -T postgres psql -U postgres renoza < backup.sql
```

## Мониторинг

### Проверка состояния контейнеров

```bash
# Статус всех контейнеров
docker-compose ps

# Использование ресурсов
docker stats renoza-postgres renoza-backend renoza-frontend

# Информация о сетях
docker network inspect renoza-network

# Информация о volumes
docker volume inspect renoza-postgres-data
```

### Health Checks

Все сервисы имеют настроенные health checks:

- **PostgreSQL**: `pg_isready`
- **Backend**: HTTP запрос на `/health`
- **Frontend**: HTTP запрос на `/health`

Проверить статус:
```bash
docker-compose ps
# или
docker inspect renoza-backend --format='{{.State.Health.Status}}'
```

## Production Deployment

Для production окружения рекомендуется:

### 1. Безопасность

```env
# Используйте сложные пароли
POSTGRES_PASSWORD=your_strong_password_here

# Не публикуйте порт PostgreSQL наружу
# Закомментируйте строку ports в postgres сервисе
```

### 2. Reverse Proxy

Добавьте nginx или traefik для SSL/TLS:

```yaml
  nginx-proxy:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf:ro
      - ./ssl:/etc/nginx/ssl:ro
    depends_on:
      - frontend
      - backend
    networks:
      - renoza-network
```

### 3. Логирование

Настройте централизованное логирование (ELK, Loki и т.д.):

```yaml
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"
```

### 4. Ресурсы

Ограничьте использование ресурсов:

```yaml
    deploy:
      resources:
        limits:
          cpus: '2'
          memory: 2G
        reservations:
          memory: 512M
```

### 5. Backup Strategy

Настройте автоматические резервные копии:

```bash
# Добавьте в cron
0 2 * * * cd /path/to/renoza/docker && docker-compose exec postgres pg_dump -U postgres renoza | gzip > /backups/renoza_$(date +\%Y\%m\%d).sql.gz
```

## Troubleshooting

### Проблема: Контейнеры не запускаются

```bash
# Проверьте логи
docker-compose logs

# Проверьте, не заняты ли порты
netstat -tuln | grep -E '5432|8080|4200'

# Проверьте права доступа
ls -la ../Renoza.Backend
ls -la ../Renoza.Frontend
```

### Проблема: Backend не может подключиться к БД

```bash
# Проверьте, что PostgreSQL запущен и healthy
docker-compose ps postgres

# Проверьте подключение вручную
docker-compose exec postgres psql -U postgres -d renoza

# Проверьте строку подключения в логах backend
docker-compose logs backend | grep "Connection"
```

### Проблема: Frontend не может подключиться к Backend

```bash
# Проверьте настройки CORS в Backend
# Проверьте конфигурацию apiUrl в Frontend

# Проверьте сеть
docker network inspect renoza-network
```

### Полная очистка и перезапуск

```bash
# Остановить и удалить все
docker-compose down -v

# Удалить образы
docker-compose down --rmi all

# Очистить volumes
docker volume prune

# Пересобрать и запустить
docker-compose build --no-cache
docker-compose up -d
```

## Обновление приложения

```bash
# 1. Остановить сервисы
docker-compose down

# 2. Получить последние изменения из git
cd ..
git pull

# 3. Пересобрать образы
cd docker
docker-compose build

# 4. Запустить обновленные сервисы
docker-compose up -d

# 5. Проверить логи
docker-compose logs -f
```

## Полезные команды

```bash
# Посмотреть все запущенные контейнеры проекта
docker-compose ps

# Очистить неиспользуемые образы
docker image prune -a

# Посмотреть размер образов
docker images | grep renoza

# Экспортировать образ
docker save renoza-backend:latest | gzip > renoza-backend.tar.gz

# Импортировать образ
docker load < renoza-backend.tar.gz
```

## Дополнительная информация

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [PostgreSQL Docker Hub](https://hub.docker.com/_/postgres)
- [ASP.NET Core Docker Documentation](https://docs.microsoft.com/aspnet/core/host-and-deploy/docker/)
