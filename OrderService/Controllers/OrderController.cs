using Microsoft.AspNetCore.Mvc;
using Orders.Application.Common.Models;
using Orders.Application.Orders.Models;
using Orders.Application.Orders.Services;

namespace OrderService.Controllers;

/// <summary>
/// Предоставляет HTTP API для работы с заказами.
/// </summary>
[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderController"/>.
    /// </summary>
    /// <param name="orderService">Сервис работы с заказами.</param>
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }


    /// <summary>
    /// Возвращает страницу заказов, начиная с самых новых.
    /// </summary>
    /// <param name="request">Номер и размер страницы.</param>
    /// <param name="cancellationToken">Токен отмены HTTP-запроса.</param>
    /// <returns>HTTP-ответ со страницей заказов.</returns>
    /// <response code="200">Страница заказов успешно получена.</response>
    /// <response code="400">Параметры запроса некорректны.</response>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<OrderViewModel>>> GetAll(
        [FromQuery] PaginatedRequest request,
        CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllAsync(request, cancellationToken);

        return Ok(orders);
    }


    /// <summary>
    /// Возвращает заказ по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="cancellationToken">Токен отмены HTTP-запроса.</param>
    /// <returns>HTTP-ответ с данными заказа или статусом отсутствия заказа.</returns>
    /// <response code="200">Заказ найден.</response>
    /// <response code="404">Заказ не найден.</response>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderViewModel>> GetOrderById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    /// <summary>
    /// Создаёт заказ.
    /// </summary>
    /// <param name="request">Данные нового заказа.</param>
    /// <param name="cancellationToken">Токен отмены HTTP-запроса.</param>
    /// <returns>
    /// HTTP-ответ с данными созданного заказа и его адресом в заголовке Location.
    /// </returns>
    /// <response code="201">Заказ создан.</response>
    /// <response code="400">Данные запроса некорректны.</response>
    [HttpPost]
    public async Task<ActionResult<OrderViewModel>> CreateOrder(
        [FromBody] OrderUpsertModel request,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.CreateOrderAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetOrderById),
            new { id = order.OrderId },
            order);
    }

    /// <summary>
    /// Обновляет данные существующего заказа.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="request">Новые данные заказа.</param>
    /// <param name="cancellationToken">Токен отмены HTTP-запроса.</param>
    /// <returns>HTTP-ответ с обновлёнными данными заказа или сообщением об ошибке.</returns>
    /// <response code="200">Заказ обновлён.</response>
    /// <response code="400">Данные запроса некорректны.</response>
    /// <response code="404">Заказ не найден.</response>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OrderViewModel>> UpdateOrderById(
        Guid id,
        [FromBody] OrderUpsertModel request,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.UpdateOrderAsync(id, request, cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    /// <summary>
    /// Удаляет заказ по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="cancellationToken">Токен отмены HTTP-запроса.</param>
    /// <returns>HTTP-ответ без содержимого при успешном удалении.</returns>
    /// <response code="204">Заказ удалён.</response>
    /// <response code="404">Заказ не найден.</response>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOrderById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _orderService.DeleteOrderAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
