using Ardalis.Specification.EntityFrameworkCore;
using Common.Application.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Orders.Application.Orders.Models;
using Orders.Application.Orders.Repositories;
using Orders.Application.Orders.Specifications;
using Orders.Domain.Orders;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure.Orders.Repositories;

/// <summary>
/// Реализует чтение и сохранение заказов с использованием Entity Framework Core.
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _dbContext;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderRepository"/>.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных заказов.</param>
    public OrderRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
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


    /// <inheritdoc />
    public Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return _dbContext.Orders
            .FirstOrDefaultAsync(
                order => order.OrderId == id,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> TryReserveDeliveryAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var affectedRows = await _dbContext.Orders
            .Where(order => order.OrderId == orderId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(order => order.DeliveryReserved, true),
                cancellationToken);

        return affectedRows == 1;
    }

    /// <inheritdoc />
    public async Task<OrderDeleteResult> TryDeleteUnreservedAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var affectedRows = await _dbContext.Orders
            .Where(order => order.OrderId == orderId && order.DeliveryReserved == false)
            .ExecuteDeleteAsync(cancellationToken);

        if (affectedRows == 1)
        {
            return OrderDeleteResult.Deleted;
        }

        var orderExists = await _dbContext.Orders
            .AnyAsync(order => order.OrderId == orderId, cancellationToken);

        return orderExists ? OrderDeleteResult.DeliveryReserved : OrderDeleteResult.NotFound;
    }

    /// <inheritdoc />
    public void Add(Order order)
    {
        _dbContext.Orders.Add(order);
    }

    /// <inheritdoc />
    public void Remove(Order order)
    {
        _dbContext.Orders.Remove(order);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
