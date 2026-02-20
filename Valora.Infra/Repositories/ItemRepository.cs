using MongoDB.Driver;
using Valora.Domain.Common.Pagination;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;
using Valora.Infra.Context;

namespace Valora.Infra.Repositories;

public class ItemRepository(IMongoDatabase database, MongoContext context) 
    : BaseRepository<Item>(database, context, "items"), IItemRepository
{
    public async Task<PaginatedList<Item>> GetByCategoryAsync(
        Guid? categoryId, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken)
    {
        var builder = Builders<Item>.Filter;
        var filter = builder.Eq(x => x.IsDeleted, false);

        if (categoryId.HasValue)
            filter &= builder.Eq(x => x.CategoryId, categoryId.Value);

        var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var items = await _collection.Find(filter)
            .SortByDescending(x => x.CreatedAt) 
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Item>(items, totalCount, page, pageSize);
    }
}