using FluentAssertions;
using NSubstitute;
using Valora.Application.UseCases.Categories.GetById;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;
using Valora.UnitTests.Abstractions;

namespace Valora.UnitTests.Application.UseCases.Categories.GetById;

public class GetCategoryByIdHandlerTests
{
    private readonly ICategoryRepository _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

    [Fact(DisplayName = "Deve retornar falha (NotFound) quando a categoria não existir no banco")]
    public async Task Handle_Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var queryId = Guid.NewGuid();
        var query = new GetCategoryByIdQuery(queryId);

        _categoryRepositoryMock.GetByIdAsync(query.Id).Returns((Category?)null);

        var expectedError = Error.NotFound(
            "Category.NotFound",
            $"A categoria com o ID '{query.Id}' não foi encontrada."
        );

        // Act
        var result = await GetCategoryByIdHandler.Handle(
            query,
            _categoryRepositoryMock,
            CancellationToken.None);

        // Assert
        result.Should().BeFailure(expectedError);
    }

    [Fact(DisplayName = "Deve retornar sucesso e mapear a categoria para o Response corretamente")]
    public async Task Handle_Should_ReturnSuccess_WhenCategoryExists()
    {
        // Arrange
        var queryId = Constants.Category.Id;
        var query = new GetCategoryByIdQuery(queryId);
        
        var category = new Category(Constants.Category.Name, Constants.Category.Description);
        category.AddField("Título Original", FieldType.Text, true);
        category.AddField("Ano de Lançamento", FieldType.Number, false);

        _categoryRepositoryMock.GetByIdAsync(query.Id).Returns(category);

        // Act
        var result = await GetCategoryByIdHandler.Handle(
            query,
            _categoryRepositoryMock,
            CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        
        var response = result.Value;
        response.Should().NotBeNull();
        
        response.Id.Should().Be(category.Id);
        response.Name.Should().Be(category.Name);
        response.Description.Should().Be(category.Description);
        
        response.Schema.Should().HaveCount(2);
        
        var firstField = response.Schema.First();
        firstField.Name.Should().Be("Título Original");
        firstField.Type.Should().Be(FieldType.Text);
        firstField.IsRequired.Should().BeTrue();
    }
}