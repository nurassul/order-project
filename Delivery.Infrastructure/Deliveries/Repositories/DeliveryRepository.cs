using Delivery.Application.Deliveries.Repositories;
using Delivery.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Infrastructure.Deliveries.Repositories;

/// <summary>
///
/// </summary>
public class DeliveryRepository : IDeliveryRepository
{
    private readonly DeliveryDbContext _dbContext;


    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DeliveryRepository"/>.
    /// </summary>
    /// <param name="dbContext"></param>
    public DeliveryRepository(DeliveryDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    /// <inheritdoc/>
    public async Task<bool> ExistByOrderIdAsync(Guid OrderId, CancellationToken token)
    {
        var exists = await _dbContext.Deliveries.AnyAsync(
            delivery => delivery.OrderId == OrderId,
            token);

        return exists;
    }


    /// <inheritdoc />
    public async Task<Domain.Models.Delivery?> GetByIdAsync(Guid deliveryId, CancellationToken cancellationToken)
    {
        return await _dbContext.Deliveries
            .AsNoTracking()
            .FirstOrDefaultAsync(delivery => delivery.DeliveryId == deliveryId, cancellationToken);
    }


    /// <inheritdoc />
    public async Task<List<Domain.Models.Delivery>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Deliveries
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }


    /// <inheritdoc />
    public void Add(Domain.Models.Delivery delivery)
    {
        _dbContext.Add(delivery);
    }


    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken token)
    {
        await _dbContext.SaveChangesAsync(token);
    }
}
