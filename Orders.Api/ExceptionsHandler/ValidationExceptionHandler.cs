using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Orders.Api.ExceptionsHandler;


/// <summary>
/// Преобразует исключения FluentValidation в HTTP-ответы
/// со статусом 400 и описанием ошибок полей.
/// </summary>
public class ValidationExceptionHandler : IExceptionHandler
{

    /// <summary>
    /// Обрабатывает исключение валидации и записывает ошибки в HTTP-ответ.
    /// </summary>
    /// <param name="httpContext">Контекст текущего HTTP-запроса.</param>
    /// <param name="exception">Исключение, возникшее при обработке запроса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// <see langword="true"/>, если исключение валидации обработано;
    /// <see langword="false"/>, если исключение имеет другой тип.
    /// </returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        var errors = validationException.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .Distinct()
                    .ToArray());

        var problem = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Ошибка валидации.",
            Instance = httpContext.Request.Path.Value,
        };

        httpContext.Response.StatusCode =
            StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(
            problem,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;


    }
}
