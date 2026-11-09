using FluentValidation;
using Orders.Application.Common.Models;
using Orders.Application.Orders.Models;
using Orders.Application.Orders.Repositories;
using Orders.Domain.Orders;

namespace Orders.Application.Orders.Services;


/// <summary>
/// Выполняет сценарии работы с заказами с использованием репозитория.
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IValidator<OrderUpsertModel> _validator;
    private readonly IValidator<PaginatedRequest> _paginationValidator;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderService"/>.
    /// </summary>
    /// <param name="repository">Репозиторий заказов.</param>
    /// <param name="validator">Валидатор данных создания и обновления заказа.</param>
    /// <param name="paginationValidator">Валидатор параметров страницы.</param>
    public OrderService(IOrderRepository repository, IValidator<OrderUpsertModel> validator, IValidator<PaginatedRequest> paginationValidator)
    {
        _repository = repository;
        _validator = validator;
        _paginationValidator = paginationValidator;
    }

    /// <inheritdoc />
    public async Task<PaginatedResult<OrderViewModel>> GetAllAsync(
        PaginatedRequest request,
        CancellationToken cancellationToken)
    {
        await _paginationValidator.ValidateAndThrowAsync(
            request, cancellationToken);

        return await _repository.GetAllAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OrderViewModel?> GetOrderByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(id, cancellationToken);

        return order is null ? null : ToViewModel(order);
    }

    /// <inheritdoc />
    public async Task<OrderViewModel> CreateOrderAsync(
        OrderUpsertModel request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            TotalAmount = request.TotalAmount,
            CreatedAt = DateTime.UtcNow,
        };

        _repository.Add(order);
        await _repository.SaveChangesAsync(cancellationToken);

        return ToViewModel(order);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteOrderAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(id, cancellationToken);

        if (order is null)
        {
            return false;
        }

        _repository.Remove(order);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task<OrderViewModel?> UpdateOrderAsync(
        Guid id,
        OrderUpsertModel request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var order = await _repository.GetByIdAsync(id, cancellationToken);

        if (order is null)
        {
            return null;
        }

        order.CustomerName = request.CustomerName;
        order.CustomerEmail = request.CustomerEmail;
        order.TotalAmount = request.TotalAmount;

        await _repository.SaveChangesAsync(cancellationToken);

        return ToViewModel(order);
    }


    private static OrderViewModel ToViewModel(Order order)
    {
        return new OrderViewModel
        {
            OrderId = order.OrderId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt
        };
    }
}
