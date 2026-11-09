namespace Orders.Application.Orders.Models;

/// <summary>
/// Содержит данные для создания или обновления заказа.
/// </summary>
public class OrderUpsertModel
{

    /// <summary>
    /// Получает или задаёт обязательное имя клиента длиной не более 100 символов.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Получает или задаёт обязательный адрес электронной почты
    /// длиной не более 100 символов.
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// Получает или задаёт сумму заказа в диапазоне от 0.01 до 999999999.99 включительно.
    /// </summary>
    public decimal TotalAmount { get; set; }
}
