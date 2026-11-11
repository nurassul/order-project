using Orders.Contracts.Orders;

namespace Orders.Application.Orders.Messaging;

/// <summary>
/// Предоставляет публикацию событий заказов
/// </summary>
public interface IOrderEventPublisher
{
    /// <summary>
    /// Публикует событие о создании заказа
    /// </summary>
    /// <param name="message">Событие с данными созданного заказа.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Задача, представляющая операцию публикации.</returns>
    Task PublishOrderCreatedAsync(OrderCreated message, CancellationToken cancellationToken);


}
