using Delivery.Domain.Models.Enums;

namespace Delivery.Domain.Models;

/// <summary>
/// Представляет заявку на доставку заказа.
/// </summary>
public class Delivery
{
    /// <summary>
    /// Возвращает или задаёт уникальный идентификатор доставки.
    /// </summary>
    public Guid DeliveryId { get; set; }

    /// <summary>
    /// Возвращает или задаёт идентификатор связанного заказа.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Возвращает или задаёт имя клиента.
    /// </summary>
    public required string CustomerName { get; set; }

    /// <summary>
    /// Возвращает или задаёт адрес электронной почты клиента.
    /// </summary>
    public required string CustomerEmail { get; set; }

    /// <summary>
    /// Возвращает или задаёт статус доставки.
    /// </summary>
    public DeliveryStatus Status { get; set; }

    /// <summary>
    /// Возвращает или задаёт дату и время создания доставки в UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
