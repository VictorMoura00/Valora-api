using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Valora.Domain.Common.Interfaces;
using Valora.Infra.Options;

namespace Valora.Infra.Context;

public class MongoContext : IUnitOfWork
{
    private readonly IMongoClient _mongoClient = null!;
    private readonly MongoSettings _settings = null!;
    private readonly List<Func<IClientSessionHandle?, Task>> _commands = new();

    protected MongoContext() { }

    public MongoContext(IMongoClient mongoClient, IOptions<MongoSettings> settings)
    {
        _mongoClient = mongoClient;
        _settings = settings.Value;
    }

    public virtual void AddCommand(Func<IClientSessionHandle?, Task> func) => _commands.Add(func);

    public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_commands.Count == 0) return; 

        if (!_settings.EnableTransactions)
        {
            foreach (var command in _commands)
            {
                await command(null);
            }
            _commands.Clear();
            return;
        }

        using var session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
        session.StartTransaction();

        try
        {
            foreach (var command in _commands)
            {
                await command(session);
            }

            await session.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await session.AbortTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _commands.Clear();
        }
    }
}