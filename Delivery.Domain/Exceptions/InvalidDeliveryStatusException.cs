namespace Delivery.Domain.Exceptions;

/// <summary>
///
/// </summary>
public class InvalidDeliveryStatusException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="InvalidDeliveryStatusException"/>.
    /// </summary>
    /// <param name="message"></param>
    public InvalidDeliveryStatusException(string message)
        : base(message)
    {
    }
}
