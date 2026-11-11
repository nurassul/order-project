namespace Orders.Contracts.Orders;

/// <summary>
/// Модель контракта создания заказа
/// </summary>
public class OrderCreated
{
    /// <summary>
    /// Возвращает или инициализирует уникальный id заказа.
    /// </summary>
    public Guid OrderId { get; init; }

    /// <summary>
    /// Возвращает или инициализирует получает имя клиента.
    /// </summary>
    public required string CustomerName { get; init; }

    /// <summary>
    /// Возвращает или инициализирует получает почту клиента.
    /// </summary>
    public required string CustomerEmail { get; init; }

    /// <summary>
    /// Возвращает или инициализирует получает общую сумму заказа
    /// </summary>
    public decimal TotalAmount { get; init; }

    /// <summary>
    /// Возвращает или инициализирует получает время создания заказ
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
