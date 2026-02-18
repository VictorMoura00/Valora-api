using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Valora.Domain.Common.Interfaces;
using Valora.Infra.Options; // Onde estiver sua MongoSettings

namespace Valora.Infra.Context;

public class MongoContext : IUnitOfWork
{
    private readonly IMongoClient _mongoClient;
    private readonly MongoSettings _settings;
    private readonly List<Func<Task>> _commands = new();

    public MongoContext(IMongoClient mongoClient, IOptions<MongoSettings> settings)
    {   
        _mongoClient = mongoClient;
        _settings = settings.Value;
    }

    public void AddCommand(Func<Task> func) => _commands.Add(func);

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        // Decisão baseada em CONFIGURAÇÃO, não em ambiente
        if (!_settings.EnableTransactions)
        {
            var commandTasks = _commands.Select(c => c());
            await Task.WhenAll(commandTasks);
            _commands.Clear();
            return;
        }

        // Lógica de Transação (Replica Set)
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