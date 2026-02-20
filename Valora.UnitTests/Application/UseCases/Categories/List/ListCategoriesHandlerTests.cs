using FluentAssertions;
using NSubstitute;
using Valora.Application.UseCases.Categories.List;
using Valora.Domain.Common.Pagination;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;
using Valora.UnitTests.Abstractions;

namespace Valora.UnitTests.Application.UseCases.Categories.List;

public class ListCategoriesHandlerTests
{
    private readonly ICategoryRepository _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

    [Fact(DisplayName = "Deve retornar sucesso com lista vazia quando não houver categorias no banco")]
    public async Task Handle_Should_ReturnSuccessWithEmptyList_WhenNoCategoriesExist()
    {
        // Arrange
        var query = new ListCategoriesQuery(Page: 1, PageSize: 10);
        
        var emptyPaginatedList = new PaginatedList<Category>(new List<Category>(), 0, query.Page, query.PageSize);

        _categoryRepositoryMock
            .GetPaginatedAsync(query.Page, query.PageSize, Arg.Any<CancellationToken>())
            .Returns(emptyPaginatedList);

        // Act
        var result = await ListCategoriesHandler.Handle(
            query, 
            _categoryRepositoryMock, 
            CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        
        var response = result.Value;
        response.Should().NotBeNull();
        response.Items.Should().BeEmpty();
        
        response.TotalCount.Should().Be(0); 
        response.PageNumber.Should().Be(query.Page);
        response.TotalPages.Should().Be(0); // 0 itens / 10 pageSize = 0 páginas
    }

    [Fact(DisplayName = "Deve retornar sucesso e mapear os itens do Domínio para DTO corretamente")]
    public async Task Handle_Should_ReturnSuccessAndMapItems_WhenCategoriesExist()
    {
        // Arrange
        var query = new ListCategoriesQuery(Page: 2, PageSize: 5);

        var category = new Category(Constants.Category.Name, Constants.Category.Description);
        category.AddField("Nota do Usuário", FieldType.Number, true);

        var categories = new List<Category> { category };
        
        var paginatedList = new PaginatedList<Category>(categories, 15, query.Page, query.PageSize);

        _categoryRepositoryMock
            .GetPaginatedAsync(query.Page, query.PageSize, Arg.Any<CancellationToken>())
            .Returns(paginatedList);

        // Act
        var result = await ListCategoriesHandler.Handle(
            query, 
            _categoryRepositoryMock, 
            CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        
        var response = result.Value;
        response.Should().NotBeNull();
        
        response.Items.Should().HaveCount(1);
        response.TotalCount.Should().Be(15);
        response.PageNumber.Should().Be(2);
        response.TotalPages.Should().Be(3);
        
        var mappedCategory = response.Items.First();
        mappedCategory.Id.Should().Be(category.Id);
        mappedCategory.Name.Should().Be(category.Name);
        mappedCategory.Description.Should().Be(category.Description);
        
        mappedCategory.Schema.Should().HaveCount(1);
        var mappedField = mappedCategory.Schema.First();
        mappedField.Name.Should().Be("Nota do Usuário");
        mappedField.Type.Should().Be(FieldType.Number);
        mappedField.IsRequired.Should().BeTrue();
    }
}