using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Valora.Domain.Common.Abstractions;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Pagination;
using Valora.Infra.Context;
using Valora.Infra.Extensions;

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

    protected FilterDefinition<T> ActiveOnlyFilter => Builders<T>.Filter.Eq(e => e.IsDeleted, false);

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var idFilter = Builders<T>.Filter.Eq(e => e.Id, id);
        var combinedFilter = Builders<T>.Filter.And(idFilter, ActiveOnlyFilter);

        return await _collection.Find(combinedFilter).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.Find(ActiveOnlyFilter).ToListAsync(cancellationToken);
    }

    public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.AddCommand(() => _collection.InsertOneAsync(entity, cancellationToken: cancellationToken));
        return Task.CompletedTask;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var idFilter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);

        _context.AddCommand(() => _collection.ReplaceOneAsync(idFilter, entity, cancellationToken: cancellationToken));
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var idFilter = Builders<T>.Filter.Eq(e => e.Id, id);

        var updateDefinition = Builders<T>.Update
            .Set(e => e.IsDeleted, true)
            .Set(e => e.UpdatedAt, DateTime.UtcNow);

        _context.AddCommand(() => _collection.UpdateOneAsync(idFilter, updateDefinition, cancellationToken: cancellationToken));
        return Task.CompletedTask;
    }

    public virtual async Task<PaginatedList<T>> GetPaginatedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _collection.ToPaginatedListAsync(
            filter: ActiveOnlyFilter,
            pageNumber: page,
            pageSize: pageSize,
            sort: null,
            cancellationToken: cancellationToken);
    }
}