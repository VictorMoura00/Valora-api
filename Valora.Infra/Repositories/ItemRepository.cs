using MongoDB.Driver;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.Infra.Repositories;

public class ItemRepository : BaseRepository<Item>, IItemRepository
{
    // Passamos o nome da coleção "items" para o construtor da base
    public ItemRepository(IMongoDatabase database) : base(database, "items")
    {
    }

    public async Task<Item?> GetByNameAsync(string name)
    {
        return await _collection .Find(x => x.Name == name && 
                                                !x.IsDeleted)
                                 .FirstOrDefaultAsync();
    }
}