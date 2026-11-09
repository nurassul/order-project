namespace Orders.Application.Orders.Models;


/// <summary>
/// Представляет данные заказа, возвращаемые приложением.
/// </summary>
public class OrderViewModel
{
    /// <summary>
    /// Получает или инициализирует уникальный id заказа.
    /// </summary>
    public Guid OrderId { get; init; }

    /// <summary>
    /// Получает или инициализирует имя клиента.
    /// </summary>
    public required string CustomerName { get; init; }

    /// <summary>
    /// Получает или инициализирует адрес электронной почты клиента.
    /// </summary>
    public required string CustomerEmail { get; init; }

    /// <summary>
    /// Получает или инициализирует общую сумму заказа.
    /// </summary>
    public decimal TotalAmount { get; init; }

    /// <summary>
    /// Получает или инициализирует дату и время создания заказа в UTC.
    /// </summary>
    public DateTime CreatedAt { get; init; }

}
