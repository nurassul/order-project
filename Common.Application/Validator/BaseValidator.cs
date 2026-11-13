using FluentValidation;

namespace Common.Application.Validator;

/// <summary>
/// Задаёт общие настройки валидаторов приложения.
/// </summary>
/// <typeparam name="T">Тип проверяемой модели.</typeparam>
public abstract class BaseValidator<T> : AbstractValidator<T>
{

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="BaseValidator{T}"/>.
    /// </summary>
    /// <remarks>
    /// После первой ошибки в цепочке правила последующие проверки
    /// этой цепочки не выполняются. Остальные правила продолжают выполняться.
    /// </remarks>
    protected BaseValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
    }
}
