namespace Orders.Application.Orders.Models;

/// <summary>
/// Описывает результат попытки удалить заказ до резервирования доставки.
/// </summary>
public enum OrderDeleteResult
{
    /// <summary>
    /// Заказ удалён.
    /// </summary>
    Deleted,

    /// <summary>
    /// Заказ с указанным идентификатором отсутствует.
    /// </summary>
    NotFound,

    /// <summary>
    /// Сервис доставки уже зарезервировал заказ, поэтому заказ сохранён.
    /// </summary>
    DeliveryReserved
}
