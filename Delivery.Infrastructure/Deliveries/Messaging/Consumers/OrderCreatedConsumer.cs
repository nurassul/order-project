using Delivery.Application.Deliveries.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orders.Contracts.Orders;

namespace Delivery.Infrastructure.Deliveries.Messaging.Consumers;

/// <summary>
///
/// </summary>
public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    private readonly IDeliveryService _deliveryService;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderCreatedConsumer"/>.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="deliveryService"></param>
    public OrderCreatedConsumer(
        ILogger<OrderCreatedConsumer> logger,
        IDeliveryService deliveryService)
    {
        _logger = logger;
        _deliveryService = deliveryService;
    }

    /// <inheritdoc/>
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var message = context.Message;

        var deliveryId = await _deliveryService.CreateForOrderAsync(
            message.OrderId,
            message.CustomerName,
            message.CustomerEmail,
            context.CancellationToken);

        if (deliveryId is null)
        {
            _logger.LogInformation(
                "Доставка для заказа {OrderId} уже существует",
                message.OrderId);
            return;
        }


        _logger.LogInformation(
            "Создана доставка {DeliveryId} для заказа {OrderId}",
            deliveryId,
            message.OrderId);
    }
}
