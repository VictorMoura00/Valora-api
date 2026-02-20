using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly List<Func<Task>> _commands = new();

    // Construtor protegido VAZIO para o NSubstitute criar o Proxy de forma segura
    protected MongoContext() 
    { 
    }

    // Construtor real utilizado pela injeção de dependência da aplicação
    public MongoContext(IMongoClient mongoClient, IOptions<MongoSettings> settings)
    {
        _mongoClient = mongoClient;
        _settings = settings.Value;
    }

    // Método virtual para que o NSubstitute consiga interceptar a chamada no teste
    public virtual void AddCommand(Func<Task> func) => _commands.Add(func);

    public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (!_settings.EnableTransactions)
        {
            var commandTasks = _commands.Select(c => c());
            await Task.WhenAll(commandTasks);
            _commands.Clear();
            return;
        }

        using var session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
        session.StartTransaction();

        try
        {
            var commandTasks = _commands.Select(c => c());
            await Task.WhenAll(commandTasks);

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