using Delivery.Application.Deliveries.Models;

namespace Delivery.Application.Deliveries.Services;

/// <summary>
///
/// </summary>
public interface IDeliveryService
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="customerName"></param>
    /// <param name="customerEmail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Guid?> CreateForOrderAsync(
        Guid orderId,
        string customerName,
        string customerEmail,
        CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    Task<List<DeliveryViewModel>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DeliveryViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

}
