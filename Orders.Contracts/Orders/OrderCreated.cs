namespace Orders.Contracts.Orders;

/// <summary>
///
/// </summary>
public class OrderCreated
{
    /// <summary>
    ///
    /// </summary>
    public Guid OrderId { get; init; }

    /// <summary>
    ///
    /// </summary>
    public required string CustomerName { get; init; }

    /// <summary>
    ///
    /// </summary>
    public required string CustomerEmail { get; init; }

    /// <summary>
    ///
    /// </summary>
    public decimal TotalAmount { get; init; }

    /// <summary>
    ///
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
