using FluentAssertions;
using MongoDB.Driver;
using NSubstitute;
using Valora.Domain.Entities;
using Valora.Infra.Context;
using Valora.Infra.Repositories;
using Xunit;

namespace Valora.UnitTests.Infra.Repositories;

public class ItemRepositoryTests
{
    [Fact(DisplayName = "Construtor deve inicializar a collection 'items' corretamente")]
    public void Constructor_Should_InitializeItemsCollection()
    {
        // Arrange
        var databaseMock = Substitute.For<IMongoDatabase>();
        var collectionMock = Substitute.For<IMongoCollection<Item>>();
        var contextMock = Substitute.For<MongoContext>();

        databaseMock.GetCollection<Item>("items", null).Returns(collectionMock);

        // Act
        var repository = new ItemRepository(databaseMock, contextMock);

        // Assert
        repository.Should().NotBeNull();
        databaseMock.Received(1).GetCollection<Item>("items", null);
    }
}
