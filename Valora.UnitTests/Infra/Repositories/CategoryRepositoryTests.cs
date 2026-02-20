using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using NSubstitute;
using Valora.Domain.Entities;
using Valora.Infra.Context;
using Valora.Infra.Repositories;
using Valora.UnitTests.Abstractions;
using Xunit;

namespace Valora.UnitTests.Infra.Repositories;

public class CategoryRepositoryTests
{
    private readonly IMongoDatabase _databaseMock;
    private readonly IMongoCollection<Category> _collectionMock;
    private readonly MongoContext _contextMock;
    private readonly CategoryRepository _repository;

    public CategoryRepositoryTests()
    {
        _collectionMock = Substitute.For<IMongoCollection<Category>>();
        _databaseMock = Substitute.For<IMongoDatabase>();
        _databaseMock.GetCollection<Category>("categories", null).Returns(_collectionMock);
        _contextMock = Substitute.For<MongoContext>();
        _repository = new CategoryRepository(_databaseMock, _contextMock);
    }

    [Fact(DisplayName = "AddAsync deve enfileirar o comando de inserção no MongoContext")]
    public async Task AddAsync_Should_AddCommandToContext()
    {
        // Arrange
        var category = new Category(Constants.Category.Name, Constants.Category.Description);

        // Act
        await _repository.AddAsync(category);

        // Assert
        _contextMock.Received(1).AddCommand(Arg.Any<Func<Task>>());
        await _collectionMock.DidNotReceiveWithAnyArgs().InsertOneAsync(default!);
    }

    [Fact(DisplayName = "UpdateAsync deve enfileirar o comando de substituição no MongoContext")]
    public async Task UpdateAsync_Should_AddCommandToContext()
    {
        // Arrange
        var category = new Category(Constants.Category.Name, Constants.Category.Description);

        // Act
        await _repository.UpdateAsync(category);

        // Assert
        _contextMock.Received(1).AddCommand(Arg.Any<Func<Task>>());
        await _collectionMock.DidNotReceiveWithAnyArgs().ReplaceOneAsync(
            Arg.Any<FilterDefinition<Category>>(), 
            Arg.Any<Category>());
    }

    [Fact(DisplayName = "DeleteAsync deve enfileirar o comando de exclusão lógica (Update) no MongoContext")]
    public async Task DeleteAsync_Should_AddCommandToContext()
    {
        // Arrange
        var categoryId = Constants.Category.Id;

        // Act
        await _repository.DeleteAsync(categoryId);

        // Assert
        _contextMock.Received(1).AddCommand(Arg.Any<Func<Task>>());
    }
}