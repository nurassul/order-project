using Deliveries.Domain.Exceptions;
using Deliveries.Domain.Models.Enums;

namespace Deliveries.Domain.Models;

/// <summary>
/// Представляет заявку на доставку заказа.
/// </summary>
public class Delivery
{
    /// <summary>
    /// Возвращает или задаёт уникальный идентификатор доставки.
    /// </summary>
    public Guid DeliveryId { get; set; }

    /// <summary>
    /// Возвращает или задаёт идентификатор связанного заказа.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Возвращает или задаёт имя клиента.
    /// </summary>
    public required string CustomerName { get; set; }

    /// <summary>
    /// Возвращает или задаёт адрес электронной почты клиента.
    /// </summary>
    public required string CustomerEmail { get; set; }

    /// <summary>
    /// Возвращает текущий статус доставки.
    /// </summary>
    public DeliveryStatus Status { get; private set; } = DeliveryStatus.Pending;

    /// <summary>
    /// Возвращает или задаёт дату и время создания доставки в UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }


    /// <summary>
    /// Переводит ожидающую доставку в статус InTransit.
    /// </summary>
    /// <exception cref="InvalidDeliveryStatusException">Доставка не находится в статусе Pending.</exception>
    public void StartDelivery()
    {
        if (Status != DeliveryStatus.Pending)
        {
            throw new InvalidDeliveryStatusException(
                "Начать можно только Pending доставку.");
        }

        Status = DeliveryStatus.InTransit;
    }

    /// <summary>
    /// Завершает доставку из статуса InTransit.
    /// </summary>
    /// <exception cref="InvalidDeliveryStatusException">Доставка не находится в статусе InTransit.</exception>
    public void ConfirmDelivery()
    {
        if (Status != DeliveryStatus.InTransit)
        {
            throw new InvalidDeliveryStatusException(
                "Можно подтвердить только InTransit доставку.");
        }

        Status = DeliveryStatus.Delivered;
    }

    /// <summary>
    /// Отменяет доставку из статуса Pending или InTransit.
    /// </summary>
    /// <exception cref="InvalidDeliveryStatusException">Доставка уже завершена или отменена.</exception>
    public void CancelDelivery()
    {
        switch (Status)
        {
            case DeliveryStatus.Cancelled:
                throw new InvalidDeliveryStatusException(
                    "Нельзя отменить Cancelled доставку.");
            case DeliveryStatus.Delivered:
                throw new InvalidDeliveryStatusException(
                    "Нельзя отменить Delivered доставку.");
        }

        Status = DeliveryStatus.Cancelled;
    }
}
