using FluentValidation;
using Orders.Application.Common.Models;

namespace Orders.Application.Common.Validation;

/// <summary>
/// Проверяет номер страницы, её размер и допустимость смещения записей.
/// </summary>
public class PaginatedRequestValidator : BaseValidator<PaginatedRequest>
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="PaginatedRequestValidator"/>.
    /// </summary>
    public PaginatedRequestValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Номер страницы должен быть не меньше 1.");

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Размер страницы должен быть от 1 до 100.");

        RuleFor(request => request)
            .Must(request =>
                ((long)request.Page - 1) * request.PageSize <= int.MaxValue)
            .WithMessage("Слишком большое смещение страницы.")
            .OverridePropertyName(nameof(PaginatedRequest.Page));
    }
}
