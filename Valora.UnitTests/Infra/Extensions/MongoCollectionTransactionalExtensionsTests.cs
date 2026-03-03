using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using NSubstitute;
using Valora.Infra.Extensions;
using Valora.UnitTests.Common.Stub.EntityStubs;
using Xunit;

namespace Valora.UnitTests.Infra.Extensions;

public class MongoCollectionTransactionalExtensionsTests
{
    private readonly IMongoCollection<EntityStub> _collectionMock = Substitute.For<IMongoCollection<EntityStub>>();
    private readonly IClientSessionHandle _sessionMock = Substitute.For<IClientSessionHandle>();
    private readonly EntityStub _document = new();

    [Fact(DisplayName = "InsertOneTransactional deve usar a sobrecarga COM sessão quando a sessão for informada")]
    public async Task InsertOneTransactional_Should_UseSession_WhenSessionIsProvided()
    {
        // Act
        await _collectionMock.InsertOneTransactionalAsync(_sessionMock, _document);

        // Assert: Verifica se a assinatura que exige IClientSessionHandle foi chamada
        await _collectionMock.Received(1).InsertOneAsync(
            _sessionMock, 
            _document, 
            Arg.Any<InsertOneOptions>(), 
            Arg.Any<CancellationToken>());
            
        // Garante que a assinatura normal (sem sessão) não foi tocada
        await _collectionMock.DidNotReceive().InsertOneAsync(
            _document, 
            Arg.Any<InsertOneOptions>(), 
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "InsertOneTransactional deve usar a sobrecarga SEM sessão quando a sessão for nula")]
    public async Task InsertOneTransactional_Should_UseNormalMethod_WhenSessionIsNull()
    {
        // Act
        await _collectionMock.InsertOneTransactionalAsync(null, _document);

        // Assert: Verifica se a assinatura normal foi chamada
        await _collectionMock.Received(1).InsertOneAsync(
            _document, 
            Arg.Any<InsertOneOptions>(), 
            Arg.Any<CancellationToken>());
            
        await _collectionMock.DidNotReceiveWithAnyArgs().InsertOneAsync(default(IClientSessionHandle)!, default!);
    }

    [Fact(DisplayName = "ReplaceOneTransactional deve usar a sobrecarga COM sessão quando a sessão for informada")]
    public async Task ReplaceOneTransactional_Should_UseSession_WhenSessionIsProvided()
    {
        // Arrange
        var filter = Builders<EntityStub>.Filter.Eq(x => x.Id, _document.Id);

        // Act
        await _collectionMock.ReplaceOneTransactionalAsync(_sessionMock, filter, _document);

        // Assert
        await _collectionMock.Received(1).ReplaceOneAsync(
            _sessionMock,
            filter,
            _document,
            Arg.Any<ReplaceOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateOneTransactional deve usar a sobrecarga SEM sessão quando a sessão for nula")]
    public async Task UpdateOneTransactional_Should_UseNormalMethod_WhenSessionIsNull()
    {
        // Arrange
        var filter = Builders<EntityStub>.Filter.Eq(x => x.Id, _document.Id);
        var update = Builders<EntityStub>.Update.Set(x => x.Id, Guid.NewGuid());

        // Act
        await _collectionMock.UpdateOneTransactionalAsync(null, filter, update);

        // Assert
        await _collectionMock.Received(1).UpdateOneAsync(
            filter,
            update,
            Arg.Any<UpdateOptions>(),
            Arg.Any<CancellationToken>());
            
        await _collectionMock.DidNotReceiveWithAnyArgs().UpdateOneAsync(default(IClientSessionHandle)!, default!, default!);
    }
}