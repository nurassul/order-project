using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Orders.Application.Common.Models;
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

        _orderRepository.Received(1).Add(Arg.Is<Order>(o => o.OrderId == res.OrderId &&
                                                            o.CustomerName == order.CustomerName &&
                                                            o.CustomerEmail == order.CustomerEmail &&
                                                            o.TotalAmount == order.TotalAmount));
    }


    /// <summary>
    /// Проверяет, что невалидный заказ вызывает ошибку валидации без обращения к добавлению и сохранению.
    /// </summary>
    [Fact]
    public async Task CreateOrderAsyncWithInvalidRequestDoesNotSaveOrder()
    {
        var order = new OrderUpsertModel
        {
            CustomerName = "Test",
            CustomerEmail = "test@mail.com",
            TotalAmount = 0,
        };

        Func<Task> action = () => _orderService.CreateOrderAsync(order, CancellationToken.None);

        await action.Should()
            .ThrowAsync<ValidationException>();

        _orderRepository.DidNotReceive().Add(Arg.Any<Order>());
        await _orderRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }


    /// <summary>
    /// Проверка удаления несуществующего заказа.
    /// </summary>
    [Fact]
    public async Task DeleteNotExistingOrder()
    {
        var orderId = Guid.NewGuid();

        _orderRepository
            .GetByIdAsync(orderId, CancellationToken.None)
            .Returns(Task.FromResult<Order?>(null));

        var res = await _orderService.DeleteOrderAsync(orderId, CancellationToken.None);

        res.Should()
            .BeFalse();

        _orderRepository.DidNotReceive().Remove(Arg.Any<Order>());
        await _orderRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }


    /// <summary>
    /// Проверка удаления существующего заказа.
    /// </summary>
    [Fact]
    public async Task DeleteExistingOrder()
    {
        var orderId = Guid.NewGuid();

        var order = new Order
        {
            OrderId = orderId,
            CustomerName = "Test",
            CustomerEmail = "test@mail.com",
            CreatedAt = DateTime.UtcNow,
            TotalAmount = 15000,
        };

        _orderRepository
            .GetByIdAsync(orderId, CancellationToken.None)
            .Returns(Task.FromResult<Order?>(order));

        var res = await _orderService.DeleteOrderAsync(orderId, CancellationToken.None);

        res.Should()
            .BeTrue();

        _orderRepository.Received(1).Remove(order);
        await _orderRepository.Received(1).SaveChangesAsync(CancellationToken.None);
    }


    /// <summary>
    /// Проверка обновления существующего заказа.
    /// </summary>
    [Fact]
    public async Task UpdateExistingOrderAsyncAndSavesOrder()
    {
        var orderId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        var orderToUpdate = new Order
        {
            OrderId = orderId,
            CustomerName = "Test",
            CustomerEmail = "test@mail.com",
            CreatedAt = createdAt,
            TotalAmount = 15000,
        };

        var request = new OrderUpsertModel
        {
            CustomerName = "Nura",
            CustomerEmail = "nura@mail.com",
            TotalAmount = 20000,
        };

        _orderRepository
            .GetByIdAsync(orderId, CancellationToken.None)
            .Returns(Task.FromResult<Order?>(orderToUpdate));

        var res = await _orderService.UpdateOrderAsync(orderId, request, CancellationToken.None);

        res.Should()
            .NotBeNull();

        res.CustomerName.Should()
            .Be(request.CustomerName);
        res.CustomerEmail.Should()
            .Be(request.CustomerEmail);
        res.TotalAmount.Should()
            .Be(request.TotalAmount);

        res.CreatedAt.Should()
            .Be(createdAt);
        res.OrderId.Should()
            .Be(orderId);

        orderToUpdate.CustomerName.Should()
            .Be(request.CustomerName);
        orderToUpdate.CustomerEmail.Should()
            .Be(request.CustomerEmail);
        orderToUpdate.TotalAmount.Should()
            .Be(request.TotalAmount);

        orderToUpdate.CreatedAt.Should()
            .Be(createdAt);
        orderToUpdate.OrderId.Should()
            .Be(orderId);

        await _orderRepository.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    /// <summary>
    /// Проверяет, что обновление отсутствующего заказа возвращает null без сохранения изменений.
    /// </summary>
    [Fact]
    public async Task UpdateNotExistingOrderAsync()
    {
        var orderId = Guid.NewGuid();

        var request = new OrderUpsertModel
        {
            CustomerName = "Nura",
            CustomerEmail = "nura@mail.com",
            TotalAmount = 20000,
        };

        _orderRepository
            .GetByIdAsync(orderId, CancellationToken.None)
            .Returns(Task.FromResult<Order?>(null));

        var res = await _orderService.UpdateOrderAsync(orderId, request, CancellationToken.None);

        res.Should()
            .BeNull();

        await _orderRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        _orderRepository.DidNotReceive().Add(Arg.Any<Order>());
    }

    /// <summary>
    /// Проверяет, что невалидное обновление вызывает ошибку валидации без изменения заказа и сохранения
    /// </summary>
    [Fact]
    public async Task UpdateOrderAsyncWithNotValidRequestAndDoesNotChangeOrder()
    {
        var orderId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        var orderToUpdate = new Order
        {
            OrderId = orderId,
            CustomerName = "Test",
            CustomerEmail = "test@mail.com",
            CreatedAt = createdAt,
            TotalAmount = 15000,
        };

        var request = new OrderUpsertModel
        {
            CustomerName = "Nura",
            CustomerEmail = "nura@mail.com",
            TotalAmount = 0,
        };

        _orderRepository
            .GetByIdAsync(orderId, CancellationToken.None)
            .Returns(Task.FromResult<Order?>(orderToUpdate));

        Func<Task> action = () => _orderService.UpdateOrderAsync(orderId, request, CancellationToken.None);

        await action.Should()
            .ThrowAsync<ValidationException>();

        orderToUpdate.CustomerName.Should()
            .Be("Test");
        orderToUpdate.CustomerEmail.Should()
            .Be("test@mail.com");
        orderToUpdate.TotalAmount.Should()
            .Be(15000);

        orderToUpdate.CreatedAt.Should()
            .Be(createdAt);
        orderToUpdate.OrderId.Should()
            .Be(orderId);

        await _orderRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }


    /// <summary>
    /// Проверка получения существующего заказа по id.
    /// </summary>
    [Fact]
    public async Task GetExistingOrderByIdAsync()
    {
        var orderId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var order = new Order
        {
            OrderId = orderId,
            CustomerName = "Test",
            CustomerEmail = "test@mail.com",
            CreatedAt = createdAt,
            TotalAmount = 15000,
        };

        _orderRepository
            .GetByIdAsync(orderId, CancellationToken.None)
            .Returns(Task.FromResult<Order?>(order));

        var res = await _orderService.GetOrderByIdAsync(orderId, CancellationToken.None);

        res.Should()
            .NotBeNull();

        res.CustomerName.Should()
            .Be(order.CustomerName);
        res.CustomerEmail.Should()
            .Be(order.CustomerEmail);
        res.TotalAmount.Should()
            .Be(order.TotalAmount);

        res.CreatedAt.Should()
            .Be(createdAt);
        res.OrderId.Should()
            .Be(orderId);
    }


    /// <summary>
    /// Проверяет, что получение отсутствующего заказа возвращает null
    /// </summary>
    [Fact]
    public async Task GetNotExistingOrderByIdAsync()
    {
        var orderId = Guid.NewGuid();

        _orderRepository
            .GetByIdAsync(orderId, CancellationToken.None)
            .Returns(Task.FromResult<Order?>(null));

        var res = await _orderService.GetOrderByIdAsync(orderId, CancellationToken.None);

        res.Should()
            .BeNull();
    }

    /// <summary>
    /// Проверка получения всех заказов с валидной пагинацией.
    /// </summary>
    [Fact]
    public async Task GetAllAsyncWithValidRequest()
    {
        var request = new PaginatedRequest
        {
            Page = 2,
            PageSize = 10,
        };

        var expectedPage = new PaginatedResult<OrderViewModel>
        {
            Page = 2,
            PageSize = 1,
            TotalCount = 3,
            Items = new List<OrderViewModel>
            {
                new OrderViewModel
                {
                    CustomerName = "Test",
                    CustomerEmail = "test@mail.com",
                    CreatedAt = DateTime.UtcNow,
                    TotalAmount = 15000,
                },
            },
        };

        _orderRepository.GetAllAsync(request, CancellationToken.None)
            .Returns(Task.FromResult<PaginatedResult<OrderViewModel>>(expectedPage));

        var result = await _orderService.GetAllAsync(request, CancellationToken.None);

        result.Should()
            .BeEquivalentTo(expectedPage);

        await _orderRepository.Received(1).GetAllAsync(Arg.Is<PaginatedRequest>(request), CancellationToken.None);
    }

    /// <summary>
    /// Проверка получения всех заказов с невалидной пагинацией.
    /// </summary>
    [Fact]
    public async Task GetAllAsyncWithNotValidRequestAndThrowsException()
    {
        var request = new PaginatedRequest
        {
            Page = 0,
            PageSize = 10,
        };



        Func<Task> action = () => _orderService.GetAllAsync(request, CancellationToken.None);

        await action.Should()
            .ThrowAsync<ValidationException>();

        await _orderRepository.DidNotReceive().GetAllAsync(Arg.Any<PaginatedRequest>(), Arg.Any<CancellationToken>());
    }







}
