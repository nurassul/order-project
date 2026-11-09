using FluentValidation;

namespace Orders.Application.Common.Validation;

public abstract class BaseValidator<T> : AbstractValidator<T> 
{
    protected BaseValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
    }
}