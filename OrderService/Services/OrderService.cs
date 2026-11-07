using Microsoft.EntityFrameworkCore;
using OrderService.Contracts;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Services;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _dbContext;

    public OrderService(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(order => order.OrderId == id);
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            TotalAmount = request.TotalAmount,
            CreatedAt = DateTime.UtcNow,
        };

        _dbContext.Orders.Add(order);

        await _dbContext.SaveChangesAsync();

        return order;
    }

    public async Task<bool> DeleteOrderAsync(Guid id)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(order => order.OrderId == id);
        if (order is null)
        {
            return false;
        }

        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Order?> UpdateOrderAsync(Guid id, UpdateOrderRequest request)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(order => order.OrderId == id);
        if (order is null)
        {
            return null;
        }

        order.CustomerEmail = request.CustomerEmail;
        order.CustomerName = request.CustomerName;
        order.TotalAmount = request.TotalAmount;

        await _dbContext.SaveChangesAsync();

        return order;
    }
}