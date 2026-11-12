namespace Common.Abstractions;

/// <summary>
/// Общий интерфейс для публикации событий.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Передаёт событие механизму публикации.
    /// </summary>
    /// <typeparam name="T">Тип события.</typeparam>
    /// <param name="message">Публикуемое событие.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Задача, представляющая операцию публикации.</returns>
    Task PublishAsync<T>(
        T message,
        CancellationToken token)
        where T : class;
}
