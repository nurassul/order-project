using Delivery.Application.Deliveries.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orders.Contracts.Orders;

namespace Delivery.Infrastructure.Deliveries.Messaging.Consumers;

/// <summary>
/// Обрабатывает событие создания заказа и передаёт создание доставки сервису приложения.
/// </summary>
public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    private readonly IDeliveryService _deliveryService;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderCreatedConsumer"/>.
    /// </summary>
    /// <param name="logger">Средство логирования обработки событий.</param>
    /// <param name="deliveryService">Сервис создания и чтения доставок.</param>
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
