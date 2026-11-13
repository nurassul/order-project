using Common.Application.Pagination;
using Delivery.Application.Deliveries.Models;
using Delivery.Application.Deliveries.Repositories;
using Delivery.Application.Deliveries.Services;
using Delivery.Domain.Models.Enums;
using FluentAssertions;
using FluentValidation;
using NSubstitute;

namespace Delivery.Application.Tests.Deliveries.Services;

/// <summary>
/// Проверяет сценарий работы сервиса доставок.
/// </summary>
public class DeliveryServiceTests
{
    private readonly IDeliveryRepository _deliveryRepository;

    private readonly DeliveryService _deliveryService;


    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DeliveryServiceTests"/>.
    /// </summary>
    public DeliveryServiceTests()
    {
        _deliveryRepository = Substitute.For<IDeliveryRepository>();
        _deliveryService = new DeliveryService(
            deliveryRepository: _deliveryRepository,
            paginatedRequestValidator: new PaginatedRequestValidator());
    }

    /// <summary>
    /// Проверка создания новой доставки.
    /// </summary>
    [Fact]
    public async Task CreateForOrderAsyncWithNewOrderCreatesDelivery()
    {
        var orderId = Guid.NewGuid();
        var customerName = "Test";
        var customerEmail = "test@mail.com";

        _deliveryRepository.ExistByOrderIdAsync(orderId, CancellationToken.None)
            .Returns(false);

        var res = await _deliveryService.CreateForOrderAsync(
            orderId,
            customerName,
            customerEmail,
            CancellationToken.None);

        res.Should()
            .NotBeNull();
        res.Should()
            .NotBe(Guid.Empty);

        _deliveryRepository.Received(1).Add(
            Arg.Is<Domain.Models.Delivery>(delivery =>
                delivery.OrderId == orderId &&
                delivery.CustomerName == customerName &&
                delivery.CustomerEmail == customerEmail &&
                delivery.DeliveryId == res &&
                delivery.Status == DeliveryStatus.Pending));

        await _deliveryRepository.Received(1).SaveChangesAsync(CancellationToken.None);
    }


    /// <summary>
    /// Проверка создания уже существующего доставки
    /// </summary>
    [Fact]
    public async Task CreateForOrderAsyncWithExistsDelivery()
    {
        var orderId = Guid.NewGuid();
        var customerName = "Test";
        var customerEmail = "test@mail.com";

        _deliveryRepository.ExistByOrderIdAsync(orderId, CancellationToken.None)
            .Returns(true);

        var res = await _deliveryService.CreateForOrderAsync(
            orderId,
            customerName,
            customerEmail,
            CancellationToken.None);

        res.Should()
            .BeNull();

        _deliveryRepository.DidNotReceive().Add(Arg.Any<Domain.Models.Delivery>());
        await _deliveryRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Проверка получения существующего заказа
    /// </summary>
    [Fact]
    public async Task GetExistingDeliveryByIdAsync()
    {
        var deliveryId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var delivery = new Domain.Models.Delivery
        {
            DeliveryId = deliveryId,
            OrderId = orderId,
            CustomerName = "Test",
            CustomerEmail = "test@mail.com",
            CreatedAt = createdAt,
            Status = DeliveryStatus.InTransit,
        };

        _deliveryRepository.GetByIdAsync(deliveryId, CancellationToken.None)
            .Returns(delivery);

        var res = await _deliveryService.GetByIdAsync(deliveryId, CancellationToken.None);

        res.Should()
            .NotBeNull();

        res.DeliveryId.Should()
            .Be(deliveryId);
        res.OrderId.Should()
            .Be(orderId);
        res.CustomerName.Should()
            .Be("Test");
        res.CustomerEmail.Should()
            .Be("test@mail.com");
        res.Status.Should()
            .Be(DeliveryStatus.InTransit);
        res.CreatedAt.Should()
            .Be(createdAt);
    }

    /// <summary>
    /// Првоерка получения несуществующено доставки
    /// </summary>
    [Fact]
    public async Task GetNotExistingDeliveryReturnsNull()
    {
        var deliveryId = Guid.NewGuid();

        _deliveryRepository.GetByIdAsync(deliveryId, CancellationToken.None)
            .Returns((Domain.Models.Delivery?)null);

        var res = await _deliveryService.GetByIdAsync(deliveryId, CancellationToken.None);

        res.Should()
            .BeNull();

        await _deliveryRepository.Received(1)
            .GetByIdAsync(deliveryId, CancellationToken.None);
    }


    /// <summary>
    /// Проверка получения доставок с валидным запросом.
    /// </summary>
    [Fact]
    public async Task GetAllAsyncWithValidRequest()
    {
        var request = new PaginatedRequest
        {
            Page = 2,
            PageSize = 2,
        };

        var expectedPage = new PaginatedResult<DeliveryViewModel>
        {
            Page = 2,
            PageSize = 2,
            TotalCount = 3,
            Items = new List<DeliveryViewModel>
            {
                new DeliveryViewModel
                {
                    CustomerName = "Test",
                    CustomerEmail = "test@mail.com",
                    Status = DeliveryStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                },
            },
        };

        _deliveryRepository.GetAllAsync(request, CancellationToken.None)
            .Returns(Task.FromResult<PaginatedResult<DeliveryViewModel>>(expectedPage));

        var result = await _deliveryService.GetAllAsync(request, CancellationToken.None);

        result.Should()
            .BeSameAs(expectedPage);

        await _deliveryRepository.Received(1)
            .GetAllAsync(request, CancellationToken.None);
    }

    /// <summary>
    /// Проверка получения всех доставок с невалидной пагинацией.
    /// </summary>
    [Fact]
    public async Task GetAllAsyncWithNotValidRequestAndThrowsException()
    {
        var request = new PaginatedRequest
        {
            Page = 0,
            PageSize = 10,
        };


        Func<Task> action = () => _deliveryService.GetAllAsync(request, CancellationToken.None);

        await action.Should()
            .ThrowAsync<ValidationException>();

        await _deliveryRepository.DidNotReceive().GetAllAsync(Arg.Any<PaginatedRequest>(), Arg.Any<CancellationToken>());
    }
}
