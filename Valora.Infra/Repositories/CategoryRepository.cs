using MongoDB.Driver;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;
using Valora.Infra.Context;

namespace Valora.Infra.Repositories;

public class CategoryRepository(IMongoDatabase database, MongoContext context) 
    : BaseRepository<Category>(database, context, "categories"), ICategoryRepository
{
    public async Task<Category?> GetByNameAsync(string name)
    {
        var filter = Builders<Category>.Filter.Eq(c => c.Name, name);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}