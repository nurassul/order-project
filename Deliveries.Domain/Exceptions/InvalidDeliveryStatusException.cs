namespace Deliveries.Domain.Exceptions;

/// <summary>
/// Представляет ошибку недопустимого изменения статуса доставки.
/// </summary>
public class InvalidDeliveryStatusException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="InvalidDeliveryStatusException"/>.
    /// </summary>
    /// <param name="message">Описание причины запрета изменения статуса.</param>
    public InvalidDeliveryStatusException(string message)
        : base(message)
    {
    }
}
