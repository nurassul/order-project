using Microsoft.AspNetCore.Mvc;
using Orders.Application.Common.Models;
using Orders.Application.Orders.Models;
using Orders.Application.Orders.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<OrderViewModel>>> GetAll(
        [FromQuery] PaginatedRequest request,
        CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllAsync(request, cancellationToken);

        return Ok(orders);
    }

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