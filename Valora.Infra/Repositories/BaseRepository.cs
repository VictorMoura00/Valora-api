using MongoDB.Driver;
using Valora.Domain.Common;
using Valora.Domain.Common.Abstractions;
using Valora.Domain.Repositories;
// <--- Corrigido (era Abstractions)

namespace Valora.Infra.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : Entity, IAggregateRoot
{
    protected readonly IMongoCollection<T> _collection;

    protected BaseRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<T>(collectionName);
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _collection.Find(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(x => !x.IsDeleted).ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public virtual async Task UpdateAsync(T entity)
    {
        await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);

        if (entity != null)
        {
            entity.Delete(); 
            await UpdateAsync(entity);
        }
    }
}