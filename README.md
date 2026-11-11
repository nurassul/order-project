# OrderProject

Учебный REST API для работы с заказами. ASP.NET Core 10, EF Core, PostgreSQL, Ocelot, RabbitMQ и MassTransit 8.

## Запуск через Docker

Нужен запущенный Docker Desktop с Linux-контейнерами. Локально устанавливать .NET для этого запуска не нужно.
Все команды выполняются из корня репозитория, где лежит `compose.yaml`.

1. Создать `.env` рядом с `compose.yaml`: скопировать `.env.example` и заполнить все четыре значения. Если `.env` уже есть, дополнить его недостающими переменными.

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
   RABBITMQ_USERNAME=orders_user
   RABBITMQ_PASSWORD=local_rabbitmq_password
   ```

   `.env` не коммитится. Все четыре значения должны быть заполнены: они нужны для запуска и подключения к PostgreSQL и RabbitMQ.

2. Собрать и запустить gateway, API, PostgreSQL и RabbitMQ:
   ```bash
   docker compose up --build -d
   ```

3. Посмотреть состояние и логи:
   ```bash
   docker compose ps
   docker compose logs -f api-gateway orderservice rabbitmq
   ```

   Дождаться запуска Orders.Api (`Now listening on`) и подключения MassTransit (`Bus started`). Миграции применяются автоматически при старте API. Первый запуск может занять некоторое время.

4. Открыть [список заказов](http://localhost:8989/api/orders). На пустой базе вернётся страница с пустым `items`.

Запросы идут через gateway на `http://localhost:8989`. Он пересылает их в `orderservice:8080` внутри Docker. У Orders.Api нет опубликованного порта на компьютере. Для такого режима gateway запускается через Compose, а не отдельно в Rider.
PostgreSQL доступен с компьютера на `localhost:5433`, база `db_orders`, логин и пароль — из `.env`.

Если запуск не получается:

- Проверить, что Docker Desktop запущен и порты `8989`, `5433`, `5672` и `15672` свободны.
- При ошибке `Set DB_USERNAME` / `Set DB_PASSWORD` проверить файл `.env`, его имя и заполнение.
- При ошибке подключения к БД посмотреть `docker compose logs db orderservice`.
- Если база или RabbitMQ уже запускались, использовать прежние логин и пароль: изменение .env не меняет существующих пользователей в сохранённых данных.
- При ошибке подключения к RabbitMQ проверить RABBITMQ_USERNAME и RABBITMQ_PASSWORD, затем посмотреть docker compose logs rabbitmq orderservice.
- Если gateway сразу после запуска возвращает ошибку, дождаться готовности Orders.Api и повторить запрос.

Остановка:
```bash
docker compose down
```
Данные остаются в Docker volume и сохраняются между запусками.

## Проверка RabbitMQ

Панель управления: http://localhost:15672. Логин и пароль — `RABBITMQ_USERNAME` и `RABBITMQ_PASSWORD` из `.env`.
Порт `5672` используется для подключения приложений к брокеру. В Docker Orders.Api подключается к `rabbitmq:5672`.

1. Создать заказ через `POST http://localhost:8989/api/orders` (пример есть в `docs/stage-1.md`).
2. В логах `orderservice` найти сообщение `Опубликовано событие OrderCreated для заказа ...`.
3. В панели RabbitMQ открыть **Exchanges** и найти `Orders.Contracts.Orders:OrderCreated`.

Событие содержит ID заказа, имя и email клиента, сумму и время создания.
Пока consumer доставки не добавлен. Успешная публикация не означает обработку события другим сервисом; без привязанной очереди событие не будет ждать будущего consumer.

Сейчас заказ сначала сохраняется в БД, затем публикуется событие. Transactional Outbox ещё не подключён: при сбое между этими операциями заказ может остаться без отправленного события. Это следующая доработка.

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
- `Orders.Infrastructure` — EF Core, репозиторий, миграции и публикация событий через MassTransit.
- `Orders.Contracts` — общие контракты событий, сейчас `OrderCreated`.
- `Orders.Application.Tests` — модульные тесты сервиса.

Примеры API: [docs/stage-1.md](docs/stage-1.md).
