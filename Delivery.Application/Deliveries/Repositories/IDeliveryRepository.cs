using Delivery.Application.Deliveries.Models;

namespace Delivery.Application.Deliveries.Repositories;

/// <summary>
/// Предоставляет операции чтения и сохранения доставок.
/// </summary>
public interface IDeliveryRepository
{
    /// <summary>
    /// Проверяет наличие доставки для указанного заказа.
    /// </summary>
    /// <param name="OrderId">Идентификатор заказа.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Задача, результат которой равен true, если доставка для заказа существует; иначе false.</returns>
    Task<bool> ExistByOrderIdAsync(
        Guid OrderId,
        CancellationToken token);

    /// <summary>
    /// Возвращает доставку по её идентификатору.
    /// </summary>
    /// <param name="deliveryId">Идентификатор доставки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Задача, результатом которой является доставка или null, если она не найдена.</returns>
    Task<Domain.Models.Delivery?> GetByIdAsync(Guid deliveryId, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает список доставок.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Задача, результатом которой является список доставок.</returns>
    Task<List<Domain.Models.Delivery>> GetAllAsync(CancellationToken cancellationToken);


    /// <summary>
    /// Добавляет доставку в контекст без сохранения в базе данных.
    /// </summary>
    /// <param name="delivery">Добавляемая доставка.</param>
    void Add(Domain.Models.Delivery delivery);

    /// <summary>
    /// Сохраняет накопленные изменения в базе данных.
    /// </summary>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Задача, представляющая сохранение изменений.</returns>
    Task SaveChangesAsync(CancellationToken token);
}
