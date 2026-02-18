using MongoDB.Driver;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;
using Valora.Infra.Context;

namespace Valora.Infra.Repositories;

public class ItemRepository : BaseRepository<Item>, IItemRepository
{
    private readonly MongoContext _context;
    public ItemRepository(IMongoDatabase database, MongoContext context) 
        : base(database, "items")
    {
        _context = context;
    }
    
    public override Task AddAsync(Item item)
    {
        _context.AddCommand(() => _collection.InsertOneAsync(item));
        return Task.CompletedTask;
    }
    
    public async Task<Item?> GetByNameAsync(string name)
    {
        return await _collection .Find(x => x.Name == name && 
                                                !x.IsDeleted)
                                 .FirstOrDefaultAsync();
    }
}