using FluentAssertions;
using MongoDB.Driver;
using NSubstitute;
using Valora.Domain.Entities;
using Valora.Infra.Context;
using Valora.Infra.Repositories;
using Xunit;

namespace Valora.UnitTests.Infra.Repositories;

public class UserRepositoryTests
{
    [Fact(DisplayName = "Construtor deve inicializar a collection 'users' corretamente")]
    public void Constructor_Should_InitializeUsersCollection()
    {
        // Arrange
        var databaseMock = Substitute.For<IMongoDatabase>();
        var collectionMock = Substitute.For<IMongoCollection<User>>();
        var contextMock = Substitute.For<MongoContext>();

        databaseMock.GetCollection<User>("users", null).Returns(collectionMock);

        // Act
        var repository = new UserRepository(databaseMock, contextMock);

        // Assert
        repository.Should().NotBeNull();
        databaseMock.Received(1).GetCollection<User>("users", null);
    }
}
