using Common.Application.Pagination;
using Orders.Application.Orders.Models;
using Orders.Domain.Orders;

namespace Orders.Application.Orders.Repositories;


/// <summary>
/// Предоставляет операции чтения и сохранения заказов.
/// </summary>
public interface IOrderRepository
{


    /// <summary>
    /// Возвращает страницу заказов, отсортированных от новых к старым.
    /// </summary>
    /// <param name="request">Проверенные параметры страницы.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Страница данных заказов с общим количеством записей.
    /// </returns>
    Task<PaginatedResult<OrderViewModel>> GetAllAsync(
        PaginatedRequest request,
        CancellationToken cancellationToken);


    /// <summary>
    /// Возвращает заказ по id.
    /// Изменения полученного заказа можно сохранить вызовом
    /// <see cref="SaveChangesAsync"/>.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Заказ или <see langword="null"/>, если он не найден.
    /// </returns>
    Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    /// <summary>
    /// Регистрирует заказ для добавления при следующем сохранении изменений.
    /// </summary>
    /// <param name="order">Добавляемый заказ.</param>
    void Add(Order order);

    /// <summary>
    /// Регистрирует заказ для удаления при следующем сохранении изменений.
    /// </summary>
    /// <param name="order">Удаляемый заказ.</param>
    void Remove(Order order);

    /// <summary>
    /// Сохраняет накопленные изменения в хранилище.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Задача, представляющая операцию сохранения.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken);

}
