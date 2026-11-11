using MassTransit;
using Microsoft.Extensions.Logging;
using Orders.Application.Orders.Messaging;
using Orders.Contracts.Orders;

namespace Orders.Infrastructure.Orders.Messaging;

/// <summary>
/// Публикация события в RabbitMQ
/// </summary>
public class MassTransitOrderEventPublisher : IOrderEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    private readonly ILogger<MassTransitOrderEventPublisher> _logger;


    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="MassTransitOrderEventPublisher"/>.
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="logger"></param>
    public MassTransitOrderEventPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<MassTransitOrderEventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    /// <summary>
    /// Публикация события в брокер и логирование события.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task PublishOrderCreatedAsync(OrderCreated message, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(message, cancellationToken);

        _logger.LogInformation(
            "Опубликовано событие OrderCreated для заказа {OrderId}",
            message.OrderId
        );
    }
}
