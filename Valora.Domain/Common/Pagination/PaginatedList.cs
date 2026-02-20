using System;
using System.Collections.Generic;

namespace Valora.Domain.Common.Pagination; // Namespace ajustado

public class PaginatedList<T>(List<T> items, long count, int pageNumber, int pageSize)
{
    public List<T> Items { get; } = items;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
    public long TotalCount { get; } = count;
    public int TotalPages { get; } = pageSize > 0 ? (int)Math.Ceiling(count / (double)pageSize) : 0;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}