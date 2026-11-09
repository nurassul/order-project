using Orders.Application.Common.Models;
using Orders.Application.Orders.Models;

namespace Orders.Application.Orders.Services;

public interface IOrderService
{
    Task<PaginatedResult<OrderViewModel>> GetAllAsync(
        PaginatedRequest request,
        CancellationToken cancellationToken);

    Task<OrderViewModel?> GetOrderByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<OrderViewModel> CreateOrderAsync(
        OrderUpsertModel request,
        CancellationToken cancellationToken);

    Task<bool> DeleteOrderAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<OrderViewModel?> UpdateOrderAsync(
        Guid id,
        OrderUpsertModel request,
        CancellationToken cancellationToken);

}