using Ardalis.Specification.EntityFrameworkCore;
using Common.Application.Pagination;
using Delivery.Application.Deliveries.Models;
using Delivery.Application.Deliveries.Repositories;
using Delivery.Application.Deliveries.Specifications;
using Delivery.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Infrastructure.Deliveries.Repositories;

/// <summary>
/// Реализует чтение и сохранение доставок с использованием Entity Framework Core.
/// </summary>
public class DeliveryRepository : IDeliveryRepository
{
    private readonly DeliveryDbContext _dbContext;


    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DeliveryRepository"/>.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных доставок.</param>
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
    public async Task<PaginatedResult<DeliveryViewModel>> GetAllAsync(PaginatedRequest request, CancellationToken cancellationToken)
    {
        var specification = new DeliveryPageSpec(request);

        var totalCount = await _dbContext.Deliveries
            .CountAsync(cancellationToken);

        var items = await _dbContext.Deliveries
            .AsNoTracking()
            .WithSpecification(specification)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<DeliveryViewModel>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        };
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
