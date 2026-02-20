using System;
using System.Collections.Generic;
using System.Linq;

namespace Valora.Domain.Common.Pagination;

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public long TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedList(IEnumerable<T> items, long count, int pageNumber, int pageSize)
    {
        Items = items.ToList().AsReadOnly();
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = pageSize > 0 ? (int)Math.Ceiling(count / (double)pageSize) : 0;
    }

    /// <summary>
    /// Transforma uma lista paginada de Entidades em uma lista paginada de DTOs.
    /// <example>
    /// <br>Como chamar o método Map para converter uma lista paginada de categorias em uma lista paginada de CategoryDto:</br>
    /// <code>
    /// var response = pagedCategories.Map(c => new CategoryDto(c.Id, c.Name));
    /// </code>
    /// </example>
    /// </summary>
    public PaginatedList<TResult> Map<TResult>(Func<T, TResult> mapFunc)
    {
        var mappedItems = Items.Select(mapFunc).ToList();
        return new PaginatedList<TResult>(mappedItems, TotalCount, PageNumber, PageSize);
    }
}