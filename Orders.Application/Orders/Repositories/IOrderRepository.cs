using Orders.Application.Common.Models;
using Orders.Application.Orders.Models;
using Orders.Domain.Orders;

namespace Orders.Application.Orders.Repositories;

public interface IOrderRepository
{
    
    Task<PaginatedResult<OrderViewModel>> GetAllAsync(
        PaginatedRequest request,
        CancellationToken cancellationToken);

    Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    void Add(Order order);

    void Remove(Order order);

    Task SaveChangesAsync(CancellationToken cancellationToken);
    
}