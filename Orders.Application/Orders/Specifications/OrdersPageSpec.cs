using Ardalis.Specification;
using Common.Application.Pagination;
using Orders.Application.Orders.Models;
using Orders.Domain.Orders;

namespace Orders.Application.Orders.Specifications;

/// <summary>
/// Описывает выборку страницы заказов с проекцией в модели ответа.
/// </summary>
/// <remarks>
/// Заказы сортируются по дате создания по убыванию,
/// затем по идентификатору по убыванию для однозначного порядка.
/// </remarks>
public class OrdersPageSpec : Specification<Order, OrderViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrdersPageSpec"/>.
    /// </summary>
    /// <param name="request">
    /// Предварительно проверенные параметры страницы.
    /// Нумерация страниц начинается с 1.
    /// </param>
    /// <exception cref="OverflowException">
    /// Вычисленное смещение записей выходит за диапазон типа <see cref="int"/>.
    /// </exception>
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
