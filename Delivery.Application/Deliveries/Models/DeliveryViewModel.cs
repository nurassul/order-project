using Delivery.Domain.Models.Enums;

namespace Delivery.Application.Deliveries.Models;

/// <summary>
/// Представляет данные доставки, возвращаемые приложением.
/// </summary>
public class DeliveryViewModel
{
    /// <summary>
    /// Возвращает или инициализирует уникальный идентификатор доставки.
    /// </summary>
    public Guid DeliveryId { get; init; }

    /// <summary>
    /// Возвращает или инициализирует идентификатор связанного заказа.
    /// </summary>
    public Guid OrderId { get; init; }

    /// <summary>
    /// Возвращает или инициализирует имя клиента.
    /// </summary>
    public required string CustomerName { get; init; }

    /// <summary>
    /// Возвращает или инициализирует адрес электронной почты клиента.
    /// </summary>
    public required string CustomerEmail { get; init; }

    /// <summary>
    /// Возвращает или инициализирует статус доставки.
    /// </summary>
    public DeliveryStatus Status { get; init; }

    /// <summary>
    /// Возвращает или инициализирует дату и время создания доставки в UTC.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
