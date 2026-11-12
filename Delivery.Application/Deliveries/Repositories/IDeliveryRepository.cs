using Delivery.Application.Deliveries.Models;

namespace Delivery.Application.Deliveries.Repositories;

/// <summary>
///
/// </summary>
public interface IDeliveryRepository
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<bool> ExistByOrderIdAsync(
        Guid OrderId,
        CancellationToken token);

    /// <summary>
    ///
    /// </summary>
    /// <param name="deliveryId"></param>
    /// <returns></returns>
    Task<Domain.Models.Delivery?> GetByIdAsync(Guid deliveryId, CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    Task<List<Domain.Models.Delivery>> GetAllAsync(CancellationToken cancellationToken);


    /// <summary>
    ///
    /// </summary>
    /// <param name="delivery"></param>
    void Add(Domain.Models.Delivery delivery);

    /// <summary>
    ///
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task SaveChangesAsync(CancellationToken token);
}
