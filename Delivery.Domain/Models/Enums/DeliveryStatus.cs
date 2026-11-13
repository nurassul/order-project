namespace Delivery.Domain.Models.Enums;

/// <summary>
/// Определяет состояние доставки.
/// </summary>
public enum DeliveryStatus
{
    /// <summary>
    /// Доставка ожидает отправки.
    /// </summary>
    Pending = 0,
    /// <summary>
    /// Заказ находится в пути.
    /// </summary>
    InTransit = 1,
    /// <summary>
    /// Заказ доставлен.
    /// </summary>
    Delivered = 2,
    /// <summary>
    /// Доставка отменена.
    /// </summary>
    Cancelled = 3
}
