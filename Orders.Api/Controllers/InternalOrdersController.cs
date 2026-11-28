using Microsoft.AspNetCore.Mvc;
using Orders.Application.Orders.Services;

namespace Orders.Api.Controllers;

/// <summary>
/// Принимает внутренние запросы сервиса доставки на резервирование заказов.
/// </summary>
[ApiController]
[Route("internal/orders")]
public class InternalOrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="InternalOrdersController"/>.
    /// </summary>
    /// <param name="orderService">Сервис заказов, который устанавливает резерв.</param>
    public InternalOrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Резервирует существующий заказ для доставки. Запрос можно повторить после сбоя.
    /// </summary>
    /// <param name="id">Идентификатор заказа из события создания заказа.</param>
    /// <param name="cancellationToken">Токен отмены запроса.</param>
    /// <returns>Результат резервирования без тела ответа.</returns>
    /// <response code="204">Резерв установлен или уже существовал.</response>
    /// <response code="404">Заказ отсутствует.</response>
    [HttpPut("{id:guid}/delivery-reservation")]
    public async Task<IActionResult> TryReserveDelivery(
        Guid id,
        CancellationToken cancellationToken)
    {
        var res = await _orderService.TryReserveDeliveryAsync(id, cancellationToken);

        return res ? NoContent() : NotFound();
    }
}
