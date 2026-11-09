namespace Orders.Domain.Orders;

/// <summary>
/// Представляет заказ клиента.
/// </summary>
public class Order
{
    /// <summary>
    /// Получает или задаёт уникальный id заказа.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Получает или задаёт имя клиента.
    /// </summary>
    public required string CustomerName { get; set; }

    /// <summary>
    /// Получает или задаёт адрес электронной почты клиента.
    /// </summary>
    public required string CustomerEmail { get; set; }

    /// <summary>
    /// Получает или задаёт общую сумму заказа.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Получает или задаёт дату и время создания заказа в UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
