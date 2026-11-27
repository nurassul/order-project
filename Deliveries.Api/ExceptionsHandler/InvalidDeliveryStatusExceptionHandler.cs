using Deliveries.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Deliveries.Api.ExceptionsHandler;

/// <summary>
/// Преобразует ошибку изменения статуса доставки в HTTP-ответ 409.
/// </summary>
public class InvalidDeliveryStatusExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Записывает описание недопустимого перехода в ответ клиенту.
    /// </summary>
    /// <param name="httpContext">Контекст текущего HTTP-запроса.</param>
    /// <param name="exception">Исключение, возникшее при обработке запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Значение true, если исключение обработано; иначе false.</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not InvalidDeliveryStatusException deliveryStatusException)
        {
            return false;
        }

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Недопустимое изменение статуса доставки.",
            Detail = deliveryStatusException.Message,
            Instance = httpContext.Request.Path.Value,
        };

        httpContext.Response.StatusCode =
            StatusCodes.Status409Conflict;

        await httpContext.Response.WriteAsJsonAsync(
            problem,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }
}
