using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Valora.Domain.Common.Pagination;

namespace Valora.Infrastructure.Extensions;


public static class MongoPaginationExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
        this IMongoCollection<T> collection,
        FilterDefinition<T>? filter,
        int pageNumber,
        int pageSize,
        SortDefinition<T>? sort = null, // Adicionado para garantir consistência
        CancellationToken cancellationToken = default)
    {
        filter ??= Builders<T>.Filter.Empty;

        var count = await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var fluentFind = collection.Find(filter);

        if (sort != null)
            fluentFind = fluentFind.Sort(sort);

        var items = await fluentFind
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, (int)count, pageNumber, pageSize);
    }
}