# OrderProject

Учебный проект на ASP.NET Core 10: заказы и доставка. EF Core, PostgreSQL, Ocelot, RabbitMQ, MassTransit 8, FluentValidation и Ardalis.Specification.

Orders сохраняет заказ и событие через Transactional Outbox. Событие отправляется в RabbitMQ, Delivery получает его и сохраняет заявку на доставку в своей БД.

## Запуск через Docker

Нужен запущенный Docker Desktop с Linux-контейнерами. Локально устанавливать .NET для этого запуска не нужно. Все команды выполняются из корня репозитория, рядом с compose.yaml.

### 1. Создать .env

Если .env ещё нет, скопировать .env.example.

PowerShell:

~~~powershell
Copy-Item .env.example .env
~~~

Git Bash / Linux / macOS:

~~~bash
cp .env.example .env
~~~

Заполнить все четыре значения. Пример для локального запуска:

~~~dotenv
DB_USERNAME=orders_user
DB_PASSWORD=local_orders_password
RABBITMQ_USERNAME=orders_user
RABBITMQ_PASSWORD=local_rabbitmq_password
~~~

.env не коммитится. Если файл уже есть, проверить его значения, не перезаписывая.

### 2. Собрать и запустить

~~~bash
docker compose config --quiet
docker compose up -d --build
~~~

Первая команда проверяет конфигурацию без вывода значений переменных.

### 3. Дождаться готовности

~~~bash
docker compose ps -a
docker compose logs -f order-service delivery-service api-gateway
~~~

Миграции применяются автоматически при старте обоих API. Дождаться запуска приложений и сообщений MassTransit о подключении. У Delivery должен появиться endpoint delivery-orders. Ctrl+C завершает просмотр логов, но не останавливает контейнеры.

Перед первым POST дождаться запуска Delivery и появления очереди delivery-orders в RabbitMQ: события, опубликованные до создания очереди и её привязок, не сохраняются для будущего consumer.

Открыть [заказы](http://localhost:8989/api/orders?page=1&pageSize=10) и [доставки](http://localhost:8989/api/deliveries?page=1&pageSize=10). На пустой базе вернутся страницы с пустым items.

## Адреса

| Компонент | С компьютера | Внутри Docker |
| --- | --- | --- |
| Gateway | http://localhost:8989 | api-gateway:8080 |
| Orders API | Через gateway | order-service:8080 |
| Delivery API | Через gateway | delivery-service:8080 |
| PostgreSQL | localhost:5433 | db:5432 |
| RabbitMQ AMQP | localhost:5672 | rabbitmq:5672 |
| RabbitMQ Management | http://localhost:15672 | rabbitmq:15672 |

HTTP-порты Orders и Delivery наружу не опубликованы. Оба API доступны через gateway.

Используется один PostgreSQL с двумя базами: db_orders для заказов и Outbox, db_delivery для доставок. Логин и пароль — DB_USERNAME и DB_PASSWORD из .env. В текущем локальном Compose начальный пользователь PostgreSQL имеет права создания БД; миграции Delivery создают отсутствующую db_delivery при первом запуске.

Вход в [панель RabbitMQ](http://localhost:15672) — RABBITMQ_USERNAME и RABBITMQ_PASSWORD из .env.

## Проверка заказа и доставки

В Postman выбрать POST, Body → raw → JSON:

~~~http
POST http://localhost:8989/api/orders
Content-Type: application/json

{
  "customerName": "Test",
  "customerEmail": "test@example.com",
  "totalAmount": 1500
}
~~~

Ожидается 201 Created. Сохранить orderId из ответа.

Доставка создаётся асинхронно и может появиться чуть позже ответа на POST. В логах delivery-service будет «Создана доставка ... для заказа ...».

~~~http
GET http://localhost:8989/api/deliveries?page=1&pageSize=10
~~~

Для GET тело не нужно. Ответ содержит items, page, pageSize, totalCount и totalPages. Найти запись с orderId созданного заказа и взять её deliveryId:

~~~http
GET http://localhost:8989/api/deliveries/{deliveryId}
~~~

Ожидается 200 с объектом. Для несуществующего корректного GUID — 404. В пути передаётся ID доставки, а не заказа.

Статусы возвращаются числами: 0 — Pending, 1 — InTransit, 2 — Delivered, 3 — Cancelled. Новая доставка получает Pending. HTTP-методов изменения статуса пока нет.

## API

| Метод | Путь | Назначение |
| --- | --- | --- |
| GET | /api/orders?page=1&pageSize=10 | Страница заказов |
| GET | /api/orders/{id} | Заказ по ID |
| POST | /api/orders | Создать заказ |
| PUT | /api/orders/{id} | Изменить заказ |
| DELETE | /api/orders/{id} | Удалить заказ |
| GET | /api/deliveries?page=1&pageSize=10 | Страница доставок |
| GET | /api/deliveries/{id} | Доставка по ID |

Для обоих списков page начинается с 1, pageSize — от 1 до 100. По умолчанию используются 1 и 10. Запросы с page=0 или pageSize=101 возвращают 400 с описанием ошибок.

Примеры операций с заказами: [docs/stage-1.md](docs/stage-1.md).

Доставка создаётся по событию OrderCreated. Изменение или удаление заказа сейчас не изменяет доставку автоматически.

## RabbitMQ и Outbox

Orders использует EF Core Bus Outbox MassTransit: заказ и исходящее сообщение сохраняются одним SaveChangesAsync в одной транзакции. Фоновый отправитель публикует сообщение в RabbitMQ.

Лог «Подготовлено событие OrderCreated» появляется до сохранения. Он не подтверждает коммит или отправку в брокер. Ответ 201 означает сохранение заказа и сообщения Outbox, но не завершение обработки в Delivery.

Delivery получает событие из очереди delivery-orders. Повтор для уже сохранённого OrderId пропускается. Уникальный индекс запрещает две доставки для одного заказа. Отдельная обработка конфликта при одновременном создании пока не реализована.

При ошибке consumer настроены три повторные попытки с интервалом две секунды. После исчерпания попыток сообщение направляется в delivery-orders_error. Перед повторной отправкой нужно устранить причину ошибки.

### Проверка Outbox

Выполнять после успешного запуска всей системы и создания очереди Delivery.

1. Остановить брокер:

   ~~~bash
   docker compose stop rabbitmq
   ~~~

2. Создать новый заказ. Ожидается 201: заказ сохраняется, доставка пока не создаётся.

3. В SQL-клиенте подключиться к db_orders на localhost:5433:

   ~~~sql
   SELECT "SequenceNumber", "MessageId", "Body"
   FROM "OutboxMessage"
   ORDER BY "SequenceNumber";
   ~~~

4. Вернуть брокер:

   ~~~bash
   docker compose start rabbitmq
   ~~~

5. Дождаться восстановления подключения. Сообщение должно уйти из Outbox, доставка — появиться в Delivery. Повторять POST не нужно.

Пока брокер выключен, ошибки подключения и таймауты отправки ожидаемы. Пустой Outbox при работающем брокере нормален: отправленные сообщения удаляются.

### Проверка накопления сообщений

После того как очередь delivery-orders уже создана:

~~~bash
docker compose stop delivery-service
~~~

Создать заказ и снова запустить Delivery:

~~~bash
docker compose start delivery-service
docker compose logs -f delivery-service
~~~

Ожидается обработка накопленного сообщения и создание доставки.

## Тесты

Для локального запуска нужен .NET SDK 10:

~~~bash
dotnet test OrderProject.sln
~~~

Отдельно:

~~~bash
dotnet test Orders.Application.Tests
dotnet test Delivery.Application.Tests
~~~

Тесты используют xUnit, NSubstitute и FluentAssertions. Проверяются сценарии сервисов: создание, чтение, повторное создание доставки и валидация пагинации; в Orders также обновление, удаление и публикация события.

Docker для модульных тестов не нужен. Подмены не проверяют реальные SQL-запросы, транзакции Outbox и доставку RabbitMQ. Сценарии ручной проверки приведены выше.

## Структура

| Проекты | Содержимое |
| --- | --- |
| ApiGateway | Маршрутизация через Ocelot |
| Orders.Api, Delivery.Api | Контроллеры, DI, запуск и обработка ошибок |
| Orders.Application, Delivery.Application | Сервисы, модели, спецификации и интерфейсы репозиториев |
| Orders.Domain, Delivery.Domain | Сущности и статусы |
| Orders.Infrastructure, Delivery.Infrastructure | EF Core, репозитории, миграции; в Delivery также consumer |
| Orders.Contracts | Контракт OrderCreated |
| Common.Abstractions | Интерфейс публикации событий |
| Common.Messaging | Общий publisher через MassTransit |
| Common.Application | Пагинация и общие валидаторы |
| Orders.Application.Tests, Delivery.Application.Tests | Модульные тесты |

## Если запуск не получается

- Проверить Docker Desktop и порты 8989, 5433, 5672, 15672.
- Проверить наличие .env и заполнение всех четырёх переменных.
- Посмотреть состояние контейнеров и ошибки:

  ~~~bash
  docker compose ps -a
  docker compose logs --tail 100 order-service delivery-service db rabbitmq
  ~~~

- После изменения переменных Compose выполнить docker compose up -d с нужным именем сервиса. Обычный restart не обновляет окружение контейнера.
- Ошибка ConnectionString property has not been initialized означает отсутствие настройки подключения. Для Orders ключ — ConnectionStrings__OrdersDb, для Delivery — ConnectionStrings__DeliveryDb.
- Изменение паролей в .env не меняет пользователей в уже инициализированных PostgreSQL и RabbitMQ. Для существующих volumes использовать прежние учётные данные.
- Если gateway сразу после старта возвращает ошибку, дождаться готовности обоих API и повторить запрос.
- Если терминал не находит dotnet, проверить установку .NET SDK 10 и PATH. Для запуска через Docker локальный dotnet не нужен.

Остановка:

~~~bash
docker compose down
~~~

Данные сохраняются в Docker volumes. Не добавлять -v, если нужно сохранить заказы, доставки и данные RabbitMQ.
