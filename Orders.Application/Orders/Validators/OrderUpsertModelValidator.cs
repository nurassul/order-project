using FluentValidation;
using Orders.Application.Common.Validation;
using Orders.Application.Orders.Models;

namespace Orders.Application.Orders.Validators;



/// <summary>
/// Проверяет данные для создания и обновления заказа:
/// имя клиента, адрес электронной почты и сумму.
/// </summary>
public class OrderUpsertModelValidator : BaseValidator<OrderUpsertModel>
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderUpsertModelValidator"/>.
    /// </summary>
    public OrderUpsertModelValidator()
    {
        RuleFor(order => order.CustomerName)
            .NotEmpty()
            .WithMessage("Укажите имя клиента")
            .MaximumLength(100)
            .WithMessage("Имя клиента не должно превышать 100 символов");

        RuleFor(order => order.CustomerEmail)
            .NotEmpty()
            .WithMessage("Укажите почту клиента")
            .MaximumLength(100)
            .WithMessage("Почта клиента не должно превышать 100 символов")
            .EmailAddress()
            .WithMessage("Укажите корректный email");

        RuleFor(order => order.TotalAmount)
            .InclusiveBetween(0.01m, 999999999.99m)
            .WithMessage("Сумма должна быть от 0.01 до 999999999.99.");

    }
}
