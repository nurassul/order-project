using OrderService.Contracts;
using OrderService.Models;

namespace OrderService.Services;

public interface IOrderService
{
    Task<List<Order>> GetAllAsync();

    Task<Order?> GetOrderByIdAsync(Guid id);
    
    Task<Order> CreateOrderAsync(CreateOrderRequest request);

    Task<bool> DeleteOrderAsync(Guid id);

    Task<Order?> UpdateOrderAsync(Guid id, UpdateOrderRequest request);

}