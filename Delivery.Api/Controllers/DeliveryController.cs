using Common.Application.Pagination;
using Delivery.Application.Deliveries.Models;
using Delivery.Application.Deliveries.Services;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.Api.Controllers;

/// <summary>
/// Предоставляет HTTP API для чтения доставок.
/// </summary>
[ApiController]
[Route("api/deliveries")]
public class DeliveryController : ControllerBase
{
    private readonly IDeliveryService _deliveryService;


    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DeliveryController"/>.
    /// </summary>
    /// <param name="deliveryService">Сервис создания и чтения доставок.</param>
    public DeliveryController(IDeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    /// <summary>
    /// Возвращает доставку по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор доставки.</param>
    /// <param name="cancellationToken">Токен отмены запроса.</param>
    /// <returns>Ответ 200 с доставкой или 404, если доставка не найдена.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DeliveryViewModel>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var res = await _deliveryService.GetByIdAsync(id, cancellationToken);

        if (res is null)
        {
            return NotFound();
        }

        return Ok(res);
    }


    /// <summary>
    /// Возвращает список доставок.
    /// </summary>
    /// <param name="request">Номер и размер страницы.</param>
    /// <param name="cancellationToken">Токен отмены запроса.</param>
    /// <returns>Ответ 200 со списком доставок.</returns>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<DeliveryViewModel>>> GetAllAsync(
        [FromQuery] PaginatedRequest request,
        CancellationToken cancellationToken)
    {
        var res = await _deliveryService.GetAllAsync(request, cancellationToken);

        return Ok(res);
    }
}
