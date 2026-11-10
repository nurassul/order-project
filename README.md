# OrderProject

Учебный REST API для работы с заказами. ASP.NET Core 10, EF Core, PostgreSQL.

## Запуск через Docker

Нужен запущенный Docker Desktop с Linux-контейнерами. Локально устанавливать .NET для этого запуска не нужно.
Все команды выполняются из корня репозитория, где лежит `compose.yaml`.

1. Создать `.env` рядом с `compose.yaml`: скопировать `.env.example` и заполнить оба значения.

   PowerShell:
   ```powershell
   Copy-Item .env.example .env
   ```

   Git Bash / Linux / macOS:
   ```bash
   cp .env.example .env
   ```

   Пример содержимого `.env` для локального запуска:
   ```dotenv
   DB_USERNAME=orders_user
   DB_PASSWORD=local_orders_password
   ```

   `.env` не коммитится. Без заполненных значений Compose не запустится.

2. Собрать и запустить gateway, API и базу:
   ```bash
   docker compose up --build -d
   ```

3. Посмотреть состояние и логи:
   ```bash
   docker compose ps
   docker compose logs -f api-gateway orderservice
   ```

   Дождаться сообщения `Now listening on`. Миграции применяются автоматически при старте API.

4. Открыть [список заказов](http://localhost:8989/api/orders). На пустой базе вернётся страница с пустым `items`.

Запросы идут через gateway на `http://localhost:8989`. Он пересылает их в `orderservice:8080` внутри Docker. У Orders.Api нет опубликованного порта на компьютере. Для такого режима gateway запускается через Compose, а не отдельно в Rider.
PostgreSQL доступен с компьютера на `localhost:5433`, база `db_orders`, логин и пароль — из `.env`.

Если запуск не получается:

- Проверить, что Docker Desktop запущен и порты `8989` и `5433` свободны.
- При ошибке `Set DB_USERNAME` / `Set DB_PASSWORD` проверить файл `.env`, его имя и заполнение.
- При ошибке подключения к БД посмотреть `docker compose logs db orderservice`.
- Если база уже запускалась, использовать прежние логин и пароль: изменение `.env` не меняет пользователя в существующей базе.

Остановка:
```bash
docker compose down
```
Данные остаются в Docker volume и сохраняются между запусками.

## Тесты

Для локального запуска тестов нужен .NET SDK 10:
```bash
dotnet test Orders.Application.Tests
```
Тесты сервиса используют подмену репозитория, поэтому PostgreSQL и Docker для них не нужны.

## Структура

- `ApiGateway` — маршрутизация запросов через Ocelot.
- `Orders.Api` — контроллеры, DI и обработка ошибок.
- `Orders.Application` — сервисы, модели запросов и ответов, валидация, интерфейсы репозиториев.
- `Orders.Domain` — сущность заказа.
- `Orders.Infrastructure` — EF Core, репозиторий, конфигурация БД и миграции.
- `Orders.Application.Tests` — модульные тесты сервиса.

Примеры API: [docs/stage-1.md](docs/stage-1.md). Запросы для Rider: [OrderService.http](Orders.Api/OrderService.http).
