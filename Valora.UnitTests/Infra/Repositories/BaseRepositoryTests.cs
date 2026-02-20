using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using NSubstitute;
using Valora.Infra.Context;
using Valora.UnitTests.Common.Stub.RepositoryStubs; 
using Xunit;

namespace Valora.UnitTests.Infra.Repositories;

public class BaseRepositoryTests
{
    private readonly IMongoDatabase _databaseMock;
    private readonly IMongoCollection<EntityStub> _collectionMock; 
    private readonly MongoContext _contextMock;
    private readonly RepositoryStub _repository; 

    public BaseRepositoryTests()
    {
        _collectionMock = Substitute.For<IMongoCollection<EntityStub>>();
        _databaseMock = Substitute.For<IMongoDatabase>();
        _databaseMock.GetCollection<EntityStub>("test_collection", null).Returns(_collectionMock);

        _contextMock = Substitute.For<MongoContext>();

        _repository = new RepositoryStub(_databaseMock, _contextMock);
    }

    [Fact(DisplayName = "AddAsync deve enfileirar o comando e chamar InsertOneAsync quando executado")]
    public async Task AddAsync_Should_EnqueueCommand_And_CallInsertOneAsync()
    {
        // Arrange
        var entity = new EntityStub { Name = "Teste Add" };
        Func<IClientSessionHandle?, Task>? capturedCommand = null;

        _contextMock.When(x => x.AddCommand(Arg.Any<Func<IClientSessionHandle?, Task>>()))
                    .Do(callInfo => capturedCommand = callInfo.Arg<Func<IClientSessionHandle?, Task>>());

        // Act
        await _repository.AddAsync(entity);

        // Assert
        capturedCommand.Should().NotBeNull("O comando deveria ter sido enfileirado no contexto.");

        await capturedCommand!.Invoke(null);

        await _collectionMock.Received(1).InsertOneAsync(
            entity,
            Arg.Any<InsertOneOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateAsync deve enfileirar o comando e chamar ReplaceOneAsync quando executado")]
    public async Task UpdateAsync_Should_EnqueueCommand_And_CallReplaceOneAsync()
    {
        // Arrange
        var entity = new EntityStub { Name = "Teste Update" };
        Func<IClientSessionHandle?, Task>? capturedCommand = null;

        _contextMock.When(x => x.AddCommand(Arg.Any<Func<IClientSessionHandle?, Task>>()))
                    .Do(callInfo => capturedCommand = callInfo.Arg<Func<IClientSessionHandle?, Task>>());

        // Act
        await _repository.UpdateAsync(entity);

        // Assert
        capturedCommand.Should().NotBeNull();

        // Simula o Unit of Work
        await capturedCommand!.Invoke(null);

        await _collectionMock.Received(1).ReplaceOneAsync(
            Arg.Any<FilterDefinition<EntityStub>>(),
            entity,
            Arg.Any<ReplaceOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "DeleteAsync deve enfileirar o comando e executar UpdateOneAsync aplicando o Soft Delete")]
    public async Task DeleteAsync_Should_EnqueueCommand_And_PerformSoftDelete()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        Func<IClientSessionHandle?, Task>? capturedCommand = null;

        _contextMock.When(x => x.AddCommand(Arg.Any<Func<IClientSessionHandle?, Task>>()))
                    .Do(callInfo => capturedCommand = callInfo.Arg<Func<IClientSessionHandle?, Task>>());

        // Act
        await _repository.DeleteAsync(entityId);

        // Assert
        capturedCommand.Should().NotBeNull();

        // Simula o Unit of Work executando a deleção
        await capturedCommand!.Invoke(null);

        await _collectionMock.Received(1).UpdateOneAsync(
            Arg.Any<FilterDefinition<EntityStub>>(),
            Arg.Any<UpdateDefinition<EntityStub>>(),
            Arg.Any<UpdateOptions>(),
            Arg.Any<CancellationToken>());
    }
}