using MongoDB.Driver;
using Valora.Domain.Common; // <--- Corrigido (era Abstractions)
using Valora.Domain.Interfaces;

namespace Valora.Infra.Repositories;

// Adicionei o "public" para que outros projetos possam herdar dele se precisar
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
        var update = Builders<T>.Update
            .Set(x => x.IsDeleted, true)
            .Set(x => x.UpdatedAt, DateTimeOffset.UtcNow);

        await _collection.UpdateOneAsync(x => x.Id == id, update);
    }
}