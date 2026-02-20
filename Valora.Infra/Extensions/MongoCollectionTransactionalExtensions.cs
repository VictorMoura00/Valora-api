using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Valora.Infra.Extensions;

/// <summary>
/// Encapsula a lógica de verificação de sessão (Transação) do driver do MongoDB,
/// mantendo os repositórios limpos (DRY).
/// </summary>
public static class MongoCollectionTransactionalExtensions
{
    public static Task InsertOneTransactionalAsync<T>(
        this IMongoCollection<T> collection,
        IClientSessionHandle? session,
        T document,
        CancellationToken cancellationToken = default)
    {
        return session != null
            ? collection.InsertOneAsync(session, document, cancellationToken: cancellationToken)
            : collection.InsertOneAsync(document, cancellationToken: cancellationToken);
    }

    public static Task ReplaceOneTransactionalAsync<T>(
        this IMongoCollection<T> collection,
        IClientSessionHandle? session,
        FilterDefinition<T> filter,
        T replacement,
        CancellationToken cancellationToken = default)
    {
        return session != null
            ? collection.ReplaceOneAsync(session, filter, replacement, cancellationToken: cancellationToken)
            : collection.ReplaceOneAsync(filter, replacement, cancellationToken: cancellationToken);
    }

    public static Task UpdateOneTransactionalAsync<T>(
        this IMongoCollection<T> collection,
        IClientSessionHandle? session,
        FilterDefinition<T> filter,
        UpdateDefinition<T> update,
        CancellationToken cancellationToken = default)
    {
        return session != null
            ? collection.UpdateOneAsync(session, filter, update, cancellationToken: cancellationToken)
            : collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }
}