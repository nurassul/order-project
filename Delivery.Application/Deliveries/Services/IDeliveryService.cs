using Common.Application.Pagination;
using Delivery.Application.Deliveries.Models;

namespace Delivery.Application.Deliveries.Services;

/// <summary>
/// Предоставляет сценарии создания и чтения доставок.
/// </summary>
public interface IDeliveryService
{
    /// <summary>
    /// Создаёт доставку для заказа, если она ещё не существует.
    /// </summary>
    /// <param name="orderId">Идентификатор заказа.</param>
    /// <param name="customerName">Имя клиента.</param>
    /// <param name="customerEmail">Адрес электронной почты клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Задача, результатом которой является идентификатор созданной доставки или null, если доставка уже существует.</returns>
    Task<Guid?> CreateForOrderAsync(
        Guid orderId,
        string customerName,
        string customerEmail,
        CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает список моделей доставки.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Задача, результатом которой является список моделей доставки.</returns>
    Task<PaginatedResult<DeliveryViewModel>> GetAllAsync(PaginatedRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает модель доставки по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор доставки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Задача, результатом которой является модель доставки или null, если она не найдена.</returns>
    Task<DeliveryViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
