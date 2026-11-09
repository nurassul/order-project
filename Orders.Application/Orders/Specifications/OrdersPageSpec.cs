using Ardalis.Specification;
using Orders.Application.Common.Models;
using Orders.Application.Orders.Models;
using Orders.Domain.Orders;

namespace Orders.Application.Orders.Specifications;

public class OrdersPageSpec : Specification<Order, OrderViewModel>
{
    public OrdersPageSpec(PaginatedRequest request)
    {
        var skip = checked((request.Page - 1) * request.PageSize);

        Query
            .OrderByDescending(order => order.CreatedAt)
            .ThenByDescending(order => order.OrderId);

        Query
            .Skip(skip)
            .Take(request.PageSize);

        Query.Select(order => new OrderViewModel
        {
            OrderId = order.OrderId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
        });
    }
}