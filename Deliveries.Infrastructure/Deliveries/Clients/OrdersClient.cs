using System.Net;

namespace Deliveries.Infrastructure.Deliveries.Clients;

/// <summary>
/// Вызывает внутренний API заказов перед созданием доставки.
/// </summary>
public class OrdersClient
{
    private HttpClient _httpClient;

    /// <summary>
    /// Инициализирует клиент сервиса заказов.
    /// </summary>
    /// <param name="httpClient">HTTP-клиент с базовым адресом Orders API.</param>
    public OrdersClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Запрашивает резерв для заказа. Повторный запрос подтверждает существующий резерв.
    /// </summary>
    /// <param name="orderId">Идентификатор заказа.</param>
    /// <param name="cancellationToken">Токен отмены запроса.</param>
    /// <returns><see langword="true"/> при ответе 204, <see langword="false"/> при ответе 404.</returns>
    /// <exception cref="InvalidOperationException">Orders API вернул неожиданный HTTP-статус.</exception>
    public async Task<bool> TryReserveDeliveryAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var path = "internal/orders/" + orderId + "/delivery-reservation";

        var res = await _httpClient.PutAsync(path, null, cancellationToken);

        switch (res.StatusCode)
        {
            case HttpStatusCode.NoContent:
                return true;
            case HttpStatusCode.NotFound:
                return false;
            default:
                throw new InvalidOperationException();
        }
    }
}
