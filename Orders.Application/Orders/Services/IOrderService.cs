using Orders.Application.Common.Models;
using Orders.Application.Orders.Models;

namespace Orders.Application.Orders.Services;

/// <summary>
/// Выполняет операции с заказами.
/// </summary>
public interface IOrderService
{

    /// <summary>
    /// Возвращает страницу заказов, отсортированных от новых к старым.
    /// </summary>
    /// <param name="request">Номер и размер страницы.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Страница заказов с общим количеством записей.</returns>
    /// <exception cref="FluentValidation.ValidationException">
    /// Параметры страницы не прошли валидацию.
    /// </exception>
    Task<PaginatedResult<OrderViewModel>> GetAllAsync(
        PaginatedRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает заказ по id.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Данные заказа или <see langword="null"/>, если заказ не найден.
    /// </returns>
    Task<OrderViewModel?> GetOrderByIdAsync(
        Guid id,
        CancellationToken cancellationToken);


    /// <summary>
    /// Создаёт и сохраняет заказ.
    /// </summary>
    /// <param name="request">Данные нового заказа.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные созданного заказа с назначенными идентификатором и датой.</returns>
    /// <exception cref="FluentValidation.ValidationException">
    /// Данные заказа не прошли валидацию.
    /// </exception>
    Task<OrderViewModel> CreateOrderAsync(
        OrderUpsertModel request,
        CancellationToken cancellationToken);


    /// <summary>
    /// Удаляет заказ по id.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// <see langword="true"/>, если заказ удалён;
    /// <see langword="false"/>, если заказ не найден.
    /// </returns>
    Task<bool> DeleteOrderAsync(
        Guid id,
        CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет данные существующего заказа.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="request">Новые данные заказа.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Данные обновлённого заказа или <see langword="null"/>,
    /// если заказ не найден.
    /// </returns>
    /// <exception cref="FluentValidation.ValidationException">
    /// Новые данные заказа не прошли валидацию.
    /// </exception>
    Task<OrderViewModel?> UpdateOrderAsync(
        Guid id,
        OrderUpsertModel request,
        CancellationToken cancellationToken);

}
