using Common.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Common.Messaging;

/// <summary>
/// Публикует события с использованием MassTransit.
/// </summary>
public class MassTransitEventPublisher : IEventPublisher
{

    private readonly IPublishEndpoint _publishEndpoint;

    private readonly ILogger<MassTransitEventPublisher> _logger;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="MassTransitEventPublisher"/>.
    /// <param name="publishEndpoint">Средство публикации событий MassTransit.</param>
    /// <param name="logger">Средство логирования.</param>
    /// </summary>
    public MassTransitEventPublisher(IPublishEndpoint publishEndpoint, ILogger<MassTransitEventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task PublishAsync<T>(T message, CancellationToken token)
        where T : class
    {
        await _publishEndpoint.Publish(message, token);

        _logger.LogInformation(
            "Подготовлено событие {EventType}",
            typeof(T).Name);
    }
}
