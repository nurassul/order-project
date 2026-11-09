using FluentAssertions;
using NSubstitute;
using Orders.Application.Common.Validation;
using Orders.Application.Orders.Models;
using Orders.Application.Orders.Repositories;
using Orders.Application.Orders.Services;
using Orders.Application.Orders.Validators;
using Orders.Domain.Orders;

namespace Orders.Application.Tests.Orders.Services;

/// <summary>
/// Проверяет сценарии работы сервиса заказов.
/// </summary>
public class OrderServiceTests
{
    private readonly IOrderRepository _orderRepository;

    private readonly OrderService _orderService;


    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderServiceTests"/>.
    /// Создаем подмену репозитория
    /// Создаем реальный сервисный объект
    /// </summary>
    public OrderServiceTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _orderService = new OrderService(
            repository: _orderRepository,
            validator: new OrderUpsertModelValidator(),
            paginationValidator: new PaginatedRequestValidator()
        );
    }

    /// <summary>
    /// Проверка создания заказа и передачу его на сохранение
    /// </summary>
    [Fact]
    public async Task CreateOrderAsyncWithValidRequestAddsAndSavesOrder()
    {
        var order = new OrderUpsertModel
        {
            CustomerName = "Test",
            CustomerEmail = "test@mail.com",
            TotalAmount = 15000,
        };


        var res = await _orderService.CreateOrderAsync(order, CancellationToken.None);

        res.CustomerName.Should()
            .Be(order.CustomerName);

        res.CustomerEmail.Should()
            .Be(order.CustomerEmail);

        res.TotalAmount.Should()
            .Be(order.TotalAmount);

        await _orderRepository.Received(1).SaveChangesAsync(CancellationToken.None);

        res.OrderId.Should()
            .NotBe(Guid.Empty);

        _orderRepository.Received(1).Add(Arg.Is<Order>(
            o => o.OrderId == res.OrderId &&
                 o.CustomerName == order.CustomerName &&
                 o.CustomerEmail == order.CustomerEmail &&
                 o.TotalAmount == order.TotalAmount));
    }
}
