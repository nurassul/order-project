namespace Common.Application.Pagination;

/// <summary>
/// Содержит параметры постраничного получения данных.
/// </summary>
public class PaginatedRequest
{
    /// <summary>
    /// Получает или задаёт номер страницы, начиная с 1.
    /// Значение по умолчанию — 1.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Получает или задаёт максимальное количество элементов на странице.
    /// Допустимые значения — от 1 до 100. Значение по умолчанию — 10.
    /// </summary>
    public int PageSize { get; set; } = 10;
}

