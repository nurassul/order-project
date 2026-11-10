# Этап 1. API заказов

Базовый адрес при запуске через Compose: `http://localhost:8989`.
Ниже примеры запросов и ожидаемых ответов по текущему коду, а не сохранённый лог прогона.
ID и дата в примерах условные. Для следующих запросов нужно брать `orderId` из ответа на создание.

## Создание

```http
POST http://localhost:8989/api/orders
Content-Type: application/json

{
  "customerName": "Nura",
  "customerEmail": "nura@example.com",
  "totalAmount": 15000
}
```

Ожидается `201 Created`, в заголовке `Location` — адрес созданного заказа.

```json
{
  "orderId": "11111111-1111-4111-8111-111111111111",
  "customerName": "Nura",
  "customerEmail": "nura@example.com",
  "totalAmount": 15000,
  "createdAt": "2026-11-10T10:00:00Z"
}
```

Идентификатор и время создания в UTC задаёт сервис.

## Получение по ID

```http
GET http://localhost:8989/api/orders/11111111-1111-4111-8111-111111111111
```

Для существующего заказа — `200 OK` и объект с теми же полями, что при создании. Для отсутствующего — `404 Not Found`.

## Список и пагинация

```http
GET http://localhost:8989/api/orders?page=1&pageSize=10
```

Пример `200 OK`, если в базе один заказ:

```json
{
  "items": [
    {
      "orderId": "11111111-1111-4111-8111-111111111111",
      "customerName": "Nura",
      "customerEmail": "nura@example.com",
      "totalAmount": 15000,
      "createdAt": "2026-11-10T10:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1
}
```

По умолчанию `page=1`, `pageSize=10`. Размер страницы — от 1 до 100.
Сортировка: сначала новые заказы; при одинаковом времени — по ID по убыванию.
Если на странице нет заказов, `items` пустой, ответ всё равно `200`.

## Обновление

```http
PUT http://localhost:8989/api/orders/11111111-1111-4111-8111-111111111111
Content-Type: application/json

{
  "customerName": "Nura Updated",
  "customerEmail": "updated@example.com",
  "totalAmount": 20000
}
```

Ожидается `200 OK`:

```json
{
  "orderId": "11111111-1111-4111-8111-111111111111",
  "customerName": "Nura Updated",
  "customerEmail": "updated@example.com",
  "totalAmount": 20000,
  "createdAt": "2026-11-10T10:00:00Z"
}
```

ID и дата создания остаются прежними. Для отсутствующего заказа с валидным телом запроса — `404`.

## Удаление

```http
DELETE http://localhost:8989/api/orders/11111111-1111-4111-8111-111111111111
```

Ожидается `204 No Content` без тела ответа. Повторное удаление или получение этого заказа — `404 Not Found`.

## Валидация

Имя и email обязательны, максимум 100 символов. Email должен иметь корректный формат.
Сумма — от `0.01` до `999999999.99`. Эти правила действуют для POST и PUT.

```http
POST http://localhost:8989/api/orders
Content-Type: application/json

{
  "customerName": "Nura",
  "customerEmail": "nura@example.com",
  "totalAmount": 0
}
```

Ожидается `400 Bad Request`, тип содержимого `application/problem+json`.
Основные поля ответа (служебные поля опущены):

```json
{
  "title": "Ошибка валидации.",
  "status": 400,
  "instance": "/api/orders",
  "errors": {
    "TotalAmount": ["Сумма должна быть от 0.01 до 999999999.99."]
  }
}
```

Невалидная пагинация тоже возвращает `400`:

```http
GET http://localhost:8989/api/orders?page=0&pageSize=10
```

В `errors.Page` будет сообщение `Номер страницы должен быть не меньше 1.`.

## Что проверить вручную

1. Создать заказ, сохранить его ID.
2. Получить его по ID и найти в списке.
3. Обновить и повторно получить — изменения должны сохраниться.
4. Создать несколько заказов, запросить страницы с `pageSize=1`, проверить порядок и `totalCount`.
5. Отправить невалидный POST и запрос с `page=0` — ожидать `400`.
6. Удалить заказ, повторно получить и удалить — ожидать `404`.

Модульные тесты сервиса запускаются командой `dotnet test Orders.Application.Tests` из корня репозитория.
Они проверяют создание, изменение, удаление, чтение и обработку невалидных запросов с подменой репозитория.
