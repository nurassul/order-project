using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Orders.Application.Common.Models;
using Orders.Application.Orders.Models;
using Orders.Application.Orders.Repositories;
using Orders.Application.Orders.Specifications;
using Orders.Domain.Orders;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure.Orders.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _dbContext;

    public OrderRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResult<OrderViewModel>> GetAllAsync(
        PaginatedRequest request,
        CancellationToken cancellationToken)
    {
        var specification = new OrdersPageSpec(request);
        
        var totalCount = await _dbContext.Orders
            .CountAsync(cancellationToken);

        var items = await _dbContext.Orders
            .AsNoTracking()
            .WithSpecification(specification)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<OrderViewModel>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        };
    }


    public Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return _dbContext.Orders
            .FirstOrDefaultAsync(order => order.OrderId == id,
                cancellationToken);
    }

    public void Add(Order order)
    {
        _dbContext.Orders.Add(order);
    }

    public void Remove(Order order)
    {
        _dbContext.Orders.Remove(order);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}