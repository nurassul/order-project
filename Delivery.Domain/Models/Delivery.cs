using Delivery.Domain.Models.Enums;

namespace Delivery.Domain.Models;

/// <summary>
///
/// </summary>
public class Delivery
{
    /// <summary>
    ///
    /// </summary>
    public Guid DeliveryId { get; set; }

    /// <summary>
    ///
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    ///
    /// </summary>
    public required string CustomerName { get; set; }

    /// <summary>
    ///
    /// </summary>
    public required string CustomerEmail { get; set; }

    /// <summary>
    ///
    /// </summary>
    public DeliveryStatus Status { get; set; }

    /// <summary>
    ///
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
