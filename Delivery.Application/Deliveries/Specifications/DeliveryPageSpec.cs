using Ardalis.Specification;
using Common.Application.Pagination;
using Delivery.Application.Deliveries.Models;

namespace Delivery.Application.Deliveries.Specifications;


/// <summary>
/// Описывает выборку страницы заказов с проекцией в модели ответа.
/// </summary>
/// <remarks>
/// Заказы сортируются по дате создания по убыванию,
/// затем по идентификатору по убыванию для однозначного порядка.
/// </remarks>
public class DeliveryPageSpec : Specification<Domain.Models.Delivery, DeliveryViewModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DeliveryPageSpec"/>.
    /// </summary>
    /// <param name="request">
    /// Предварительно проверенные параметры страницы.
    /// Нумерация страниц начинается с 1.
    /// </param>
    public DeliveryPageSpec(PaginatedRequest request)
    {
        var skip = checked((request.Page - 1) * request.PageSize);

        Query
            .OrderByDescending(delivery => delivery.CreatedAt)
            .ThenByDescending(delivery => delivery.OrderId);

        Query
            .Skip(skip)
            .Take(request.PageSize);

        Query.Select(delivery => new DeliveryViewModel
        {
            DeliveryId = delivery.DeliveryId,
            OrderId = delivery.OrderId,
            CustomerName = delivery.CustomerName,
            CustomerEmail = delivery.CustomerEmail,
            Status = delivery.Status,
            CreatedAt = delivery.CreatedAt,
        });
    }
}
