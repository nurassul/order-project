using FluentValidation;
using Orders.Application.Common.Models;

namespace Orders.Application.Common.Validation;

public class PaginatedRequestValidator : BaseValidator<PaginatedRequest>
{
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