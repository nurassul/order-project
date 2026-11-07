using Microsoft.AspNetCore.Mvc;
using OrderService.Contracts;
using OrderService.Models;
using OrderService.Services;

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
    public async Task<ActionResult<List<Order>>> GetAll()
    {
        var orders = await _orderService.GetAllAsync();

        return Ok(orders);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Order>> GetOrderById(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order is null)
        {
            return NotFound();
        }
        
        return Ok(order);
    }
    
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(
        [FromBody] CreateOrderRequest request)
    {
        var order = await _orderService.CreateOrderAsync(request);

        return CreatedAtAction(
            nameof(GetOrderById),
            new { id = order.OrderId },
            order);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Order>> UpdateOrderById(
        [FromBody] UpdateOrderRequest request, Guid id)
    {
        var order = await _orderService.UpdateOrderAsync(id, request);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOrderById(Guid id)
    {
        var deleted = await _orderService.DeleteOrderAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    } 
    
}