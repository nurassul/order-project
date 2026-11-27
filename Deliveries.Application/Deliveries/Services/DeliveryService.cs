using Common.Abstractions;
using Common.Application.Pagination;
using Deliveries.Application.Deliveries.Models;
using Deliveries.Application.Deliveries.Repositories;
using Deliveries.Domain.Models;
using FluentValidation;

namespace Deliveries.Application.Deliveries.Services;

/// <summary>
/// Выполняет сценарии создания, чтения и изменения статусов доставок через репозиторий.
/// </summary>
public class DeliveryService : IDeliveryService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IValidator<PaginatedRequest> _paginatedRequestValidator;
    private readonly IEventPublisher _eventPublisher;


    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DeliveryService"/>.
    /// </summary>
    /// <param name="deliveryRepository">Репозиторий доставок.</param>
    /// <param name="paginatedRequestValidator">Валидатор пагинации.</param>
    /// <param name="eventPublisher">Публикатор событий.</param>
    public DeliveryService(
        IDeliveryRepository deliveryRepository,
        IValidator<PaginatedRequest> paginatedRequestValidator,
        IEventPublisher eventPublisher)
    {
        _deliveryRepository = deliveryRepository;
        _paginatedRequestValidator = paginatedRequestValidator;
        _eventPublisher = eventPublisher;
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

        var delivery = new Delivery
        {
            DeliveryId = Guid.NewGuid(),
            OrderId = orderId,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            CreatedAt = DateTime.UtcNow,
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

    /// <inheritdoc />
    public async Task<DeliveryViewModel?> StartDelivery(Guid id, CancellationToken cancellationToken)
    {
        await _deliveryRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var order = await _deliveryRepository.GetByIdForUpdateAsync(id, cancellationToken);

            if (order is null)
            {
                await _deliveryRepository.RollbackTransactionAsync(cancellationToken);

                return null;
            }

            order.StartDelivery();

            await _deliveryRepository.SaveChangesAsync(cancellationToken);

            await _deliveryRepository.CommitTransactionAsync(cancellationToken);

            return ToViewModel(order);
        }
        catch
        {
            await _deliveryRepository.RollbackTransactionAsync(CancellationToken.None);

            throw;
        }
    }

    /// <inheritdoc />
    public async Task<DeliveryViewModel?> ConfirmDelivery(Guid id, CancellationToken cancellationToken)
    {
        await _deliveryRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var order = await _deliveryRepository.GetByIdForUpdateAsync(id, cancellationToken);

            if (order is null)
            {
                await _deliveryRepository.RollbackTransactionAsync(cancellationToken);

                return null;
            }

            order.ConfirmDelivery();

            await _deliveryRepository.SaveChangesAsync(cancellationToken);

            await _deliveryRepository.CommitTransactionAsync(cancellationToken);

            return ToViewModel(order);
        }
        catch
        {
            await _deliveryRepository.RollbackTransactionAsync(CancellationToken.None);

            throw;
        }
    }

    /// <inheritdoc />
    public async Task<DeliveryViewModel?> CancelDelivery(Guid id, CancellationToken cancellationToken)
    {
        await _deliveryRepository.BeginTransactionAsync(cancellationToken);
        try
        {
            var order = await _deliveryRepository.GetByIdForUpdateAsync(id, cancellationToken);

            if (order is null)
            {
                await _deliveryRepository.RollbackTransactionAsync(cancellationToken);

                return null;
            }

            order.CancelDelivery();

            await _deliveryRepository.SaveChangesAsync(cancellationToken);

            await _deliveryRepository.CommitTransactionAsync(cancellationToken);

            return ToViewModel(order);
        }
        catch
        {
            await _deliveryRepository.RollbackTransactionAsync(CancellationToken.None);

            throw;
        }
    }


    private static DeliveryViewModel ToViewModel(Delivery delivery)
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
