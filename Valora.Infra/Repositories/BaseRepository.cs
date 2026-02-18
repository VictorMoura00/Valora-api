using MongoDB.Driver;
using Valora.Domain.Common.Abstractions;
using Valora.Domain.Common.Interfaces;
using Valora.Infra.Context; 

namespace Valora.Infra.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : Entity, IAggregateRoot
{
    protected readonly IMongoCollection<T> _collection;
    protected readonly MongoContext _context;

    protected BaseRepository(IMongoDatabase database, MongoContext context, string collectionName)
    {
        _collection = database.GetCollection<T>(collectionName);
        _context = context;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _collection
            .Find(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection
            .Find(x => !x.IsDeleted)
            .ToListAsync();
    }

    public virtual Task AddAsync(T entity)
    {
        _context.AddCommand(() => _collection.InsertOneAsync(entity));
        return Task.CompletedTask;
    }

    public virtual Task UpdateAsync(T entity)
    {
        _context.AddCommand(() => _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity));
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(Guid id)
    {
        _context.AddCommand(async () => 
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                entity.Delete();
                await _collection.ReplaceOneAsync(x => x.Id == id, entity);
            }
        });
        
        return Task.CompletedTask;
    }
}