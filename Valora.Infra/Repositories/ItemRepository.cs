using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Valora.Domain.Common.Pagination;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;
using Valora.Infra.Context;
using Valora.Infra.Extensions;

namespace Valora.Infra.Repositories;

public class ItemRepository : BaseRepository<Item>, IItemRepository
{
    public ItemRepository(IMongoDatabase database, MongoContext context)
        : base(database, context, "items")
    {
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid categoryId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Item>.Filter.And(
            ActiveOnlyFilter,
            Builders<Item>.Filter.Eq(x => x.CategoryId, categoryId),

            Builders<Item>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression($"^{name}$", "i"))
        );

        return await _collection.Find(filter).AnyAsync(cancellationToken);
    }

    public async Task<PaginatedList<Item>> GetPaginatedByCategoryAsync(
        Guid categoryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Item>.Filter.And(
            ActiveOnlyFilter,
            Builders<Item>.Filter.Eq(x => x.CategoryId, categoryId)
        );

        return await _collection.ToPaginatedListAsync(
            filter: filter,
            pageNumber: page,
            pageSize: pageSize,
            sort: Builders<Item>.Sort.Ascending(x => x.Name),
            cancellationToken: cancellationToken);
    }
}
