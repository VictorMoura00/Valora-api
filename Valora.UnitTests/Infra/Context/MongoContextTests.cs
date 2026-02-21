using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Valora.Infra.Context;
using Valora.Infra.Options;
using Xunit;

namespace Valora.UnitTests.Infra.Context;

public class MongoContextTests
{
    private readonly IMongoClient _mongoClientMock;
    private readonly IClientSessionHandle _sessionMock;
    private MongoSettings _settings;

    public MongoContextTests()
    {
        _mongoClientMock = Substitute.For<IMongoClient>();
        _sessionMock = Substitute.For<IClientSessionHandle>();

        // Configura o Client falso para retornar a Sessão falsa quando chamado
        _mongoClientMock.StartSessionAsync(
            Arg.Any<ClientSessionOptions>(), 
            Arg.Any<CancellationToken>())
            .Returns(_sessionMock);

        _settings = new MongoSettings 
        { 
            DatabaseName = "TestDB", 
            EnableTransactions = true 
        };
    }

    private MongoContext CreateContext()
    {
        var options = Options.Create(_settings);
        return new MongoContext(_mongoClientMock, options);
    }

    [Fact(DisplayName = "CommitAsync deve retornar imediatamente se não houver comandos na fila")]
    public async Task CommitAsync_Should_ReturnImmediately_WhenNoCommands()
    {
        // Arrange
        var context = CreateContext();

        // Act
        await context.CommitAsync();

        // Assert
        await _mongoClientMock.DidNotReceiveWithAnyArgs().StartSessionAsync();
    }

    [Fact(DisplayName = "CommitAsync deve executar comandos sem sessão quando transações estiverem desabilitadas")]
    public async Task CommitAsync_Should_ExecuteWithoutSession_WhenTransactionsAreDisabled()
    {
        // Arrange
        _settings.EnableTransactions = false;
        var context = CreateContext();

        bool commandExecuted = false;
        IClientSessionHandle? receivedSession = null;

        context.AddCommand(session =>
        {
            commandExecuted = true;
            receivedSession = session;
            return Task.CompletedTask;
        });

        // Act
        await context.CommitAsync();

        // Assert
        commandExecuted.Should().BeTrue();
        receivedSession.Should().BeNull("Como as transações estão desabilitadas, a sessão passada deve ser nula.");
        
        await _mongoClientMock.DidNotReceiveWithAnyArgs().StartSessionAsync();
    }

    [Fact(DisplayName = "CommitAsync deve executar o fluxo transacional completo quando habilitado e houver sucesso")]
    public async Task CommitAsync_Should_ExecuteFullTransaction_WhenEnabledAndSuccessful()
    {
        // Arrange
        _settings.EnableTransactions = true;
        var context = CreateContext();

        bool commandExecuted = false;
        IClientSessionHandle? receivedSession = null;

        context.AddCommand(session =>
        {
            commandExecuted = true;
            receivedSession = session;
            return Task.CompletedTask;
        });

        // Act
        await context.CommitAsync();

        // Assert
        commandExecuted.Should().BeTrue();
        receivedSession.Should().NotBeNull("O comando deveria ter recebido a sessão do Mongo.");
        receivedSession.Should().Be(_sessionMock); // Garante que é a mesma sessão criada

        await _mongoClientMock.Received(1).StartSessionAsync(cancellationToken: Arg.Any<CancellationToken>());
        _sessionMock.Received(1).StartTransaction(Arg.Any<TransactionOptions>());
        await _sessionMock.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
        await _sessionMock.DidNotReceiveWithAnyArgs().AbortTransactionAsync();
    }

    [Fact(DisplayName = "CommitAsync deve abortar a transação (Rollback) se um comando lançar exceção")]
    public async Task CommitAsync_Should_AbortTransaction_WhenCommandThrowsException()
    {
        // Arrange
        _settings.EnableTransactions = true;
        var context = CreateContext();
        var expectedException = new InvalidOperationException("Erro simulado no banco.");

        // Adiciona um comando que intencionalmente falha
        context.AddCommand(session => throw expectedException);

        // Act
        Func<Task> action = async () => await context.CommitAsync();

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Erro simulado no banco.");

        _sessionMock.Received(1).StartTransaction(Arg.Any<TransactionOptions>());
        await _sessionMock.Received(1).AbortTransactionAsync(Arg.Any<CancellationToken>());
        await _sessionMock.DidNotReceiveWithAnyArgs().CommitTransactionAsync();
    }

    [Fact(DisplayName = "CommitAsync deve limpar a fila de comandos após a execução, mesmo com erros")]
    public async Task CommitAsync_Should_ClearCommandsQueue_AfterExecution()
    {
        // Arrange
        _settings.EnableTransactions = true;
        var context = CreateContext();
        int executionCount = 0;

        context.AddCommand(session =>
        {
            executionCount++;
            return Task.CompletedTask;
        });

        // Act
        await context.CommitAsync(); 
        await context.CommitAsync(); 

        // Assert
        executionCount.Should().Be(1, "O comando deveria ter sido executado apenas na primeira chamada.");
    }
}