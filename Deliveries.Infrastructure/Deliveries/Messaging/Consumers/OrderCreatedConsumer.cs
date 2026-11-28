using Deliveries.Application.Deliveries.Services;
using Deliveries.Infrastructure.Deliveries.Clients;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orders.Contracts.Orders;

namespace Deliveries.Infrastructure.Deliveries.Messaging.Consumers;

/// <summary>
/// Запрашивает резерв по событию создания заказа и создаёт доставку при подтверждённом резерве.
/// Если заказ уже удалён, завершает обработку события.
/// </summary>
public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    private readonly IDeliveryService _deliveryService;

    private OrdersClient _ordersClient;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderCreatedConsumer"/>.
    /// </summary>
    /// <param name="logger">Средство логирования обработки событий.</param>
    /// <param name="deliveryService">Сервис создания и чтения доставок.</param>
    /// <param name="ordersClient">Клиент резервирования заказа в Orders API.</param>
    public OrderCreatedConsumer(
        ILogger<OrderCreatedConsumer> logger,
        IDeliveryService deliveryService,
        OrdersClient ordersClient)
    {
        _logger = logger;
        _deliveryService = deliveryService;
        _ordersClient = ordersClient;
    }

    /// <inheritdoc/>
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var message = context.Message;

        var reserved = await _ordersClient.TryReserveDeliveryAsync(message.OrderId, context.CancellationToken);

        if (!reserved)
        {
            _logger.LogInformation(
                "Заказ {OrderId} уже удалён, доставка пропущена",
                message.OrderId);
            return;
        }

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
