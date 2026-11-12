using Delivery.Domain.Models.Enums;

namespace Delivery.Application.Deliveries.Models;

/// <summary>
///
/// </summary>
public class DeliveryViewModel
{
    /// <summary>
    ///
    /// </summary>
    public Guid DeliveryId { get; init; }

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
    public DeliveryStatus Status { get; init; }

    /// <summary>
    ///
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
