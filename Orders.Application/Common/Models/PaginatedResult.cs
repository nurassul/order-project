namespace Orders.Application.Common.Models;


/// <summary>
/// Представляет страницу данных и сведения об общем количестве записей.
/// </summary>
/// <typeparam name="T">Тип элемента страницы.</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// Получает или инициализирует элементы текущей страницы.
    /// </summary>
    public required List<T> Items { get; init; }

    /// <summary>
    /// Получает или инициализирует номер страницы, начиная с 1.
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Получает или инициализирует максимальное количество элементов на странице.
    /// Значение должно быть положительным.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Получает или инициализирует общее количество записей до применения пагинации.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Получает общее количество страниц.
    /// При отсутствии записей возвращает 0.
    /// </summary>
    public int TotalPages =>
        (int)Math.Ceiling((double)TotalCount / PageSize);
}
