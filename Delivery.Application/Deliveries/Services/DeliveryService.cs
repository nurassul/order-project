using Common.Application.Pagination;
using Delivery.Application.Deliveries.Models;
using Delivery.Application.Deliveries.Repositories;
using Delivery.Domain.Models.Enums;
using FluentValidation;

namespace Delivery.Application.Deliveries.Services;

/// <summary>
/// Выполняет сценарии создания и чтения доставок через репозиторий.
/// </summary>
public class DeliveryService : IDeliveryService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IValidator<PaginatedRequest> _paginatedRequestValidator;


    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DeliveryService"/>.
    /// </summary>
    /// <param name="deliveryRepository">Репозиторий доставок.</param>
    /// <param name="paginatedRequestValidator">Валидатор пагинации.</param>
    public DeliveryService(IDeliveryRepository deliveryRepository, IValidator<PaginatedRequest> paginatedRequestValidator)
    {
        _deliveryRepository = deliveryRepository;
        _paginatedRequestValidator = paginatedRequestValidator;
    }


    /// <inheritdoc/>
    public async Task<Guid?> CreateForOrderAsync(
        Guid orderId,
        string customerName,
        string customerEmail,
        CancellationToken cancellationToken)
    {
        var exists = await _deliveryRepository.ExistByOrderIdAsync(orderId, cancellationToken);

        if (exists)
        {
            return null;
        }

        var delivery = new Domain.Models.Delivery
        {
            DeliveryId = Guid.NewGuid(),
            OrderId = orderId,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            CreatedAt = DateTime.UtcNow,
            Status = DeliveryStatus.Pending,
        };

        _deliveryRepository.Add(delivery);

        await _deliveryRepository.SaveChangesAsync(cancellationToken);

        return delivery.DeliveryId;
    }


    /// <inheritdoc/>
    public async Task<PaginatedResult<DeliveryViewModel>> GetAllAsync(
        PaginatedRequest request,
        CancellationToken cancellationToken)
    {
        await _paginatedRequestValidator.ValidateAndThrowAsync(
            request, cancellationToken);

        return await _deliveryRepository.GetAllAsync(request, cancellationToken);
    }


    /// <inheritdoc/>
    public async Task<DeliveryViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var res = await _deliveryRepository.GetByIdAsync(id, cancellationToken);

        return res is null ? null : ToViewModel(res);
    }

    private static DeliveryViewModel ToViewModel(Domain.Models.Delivery delivery)
    {
        return new DeliveryViewModel
        {
            DeliveryId = delivery.DeliveryId,
            OrderId = delivery.OrderId,
            CustomerName = delivery.CustomerName,
            CustomerEmail = delivery.CustomerEmail,
            CreatedAt = delivery.CreatedAt,
            Status = delivery.Status,
        };
    }
}
