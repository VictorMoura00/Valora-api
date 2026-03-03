using FluentAssertions;
using NSubstitute;
using Valora.Application.UseCases.Items.ListByCategory;
using Valora.Domain.Common.Pagination;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.UnitTests.Application.UseCases.Items.ListByCategory;

public class ListItemsByCategoryHandlerTests
{
    private readonly ICategoryRepository _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
    private readonly IItemRepository _itemRepositoryMock = Substitute.For<IItemRepository>();

    [Fact(DisplayName = "Deve retornar falha (NotFound) se a categoria solicitada não existir")]
    public async Task Handle_Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var query = new ListItemsByCategoryQuery(Guid.NewGuid());

        _categoryRepositoryMock.GetByIdAsync(query.CategoryId, Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        // Act
        var result = await ListItemsByCategoryHandler.Handle(
            query,
            _categoryRepositoryMock,
            _itemRepositoryMock,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Category.NotFound");

        await _itemRepositoryMock.DidNotReceiveWithAnyArgs().GetPaginatedByCategoryAsync(default, default, default, default);
    }

    [Fact(DisplayName = "Deve retornar sucesso e a lista paginada de DTOs quando a categoria existir")]
    public async Task Handle_Should_ReturnSuccess_And_MappedPaginatedList_WhenCategoryExists()
    {
        // Arrange
        var category = new Category("Jogos", "Catálogo de jogos");
        var query = new ListItemsByCategoryQuery(category.Id, PageNumber: 1, PageSize: 10);

        var item = new Item(category.Id, "The Witcher 3");
        item.SetAttribute("Plataforma", "PC");

        var paginatedItems = new PaginatedList<Item>(
            items: new List<Item> { item },
            count: 1,
            pageNumber: 1,
            pageSize: 10);

        _categoryRepositoryMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(category);

        _itemRepositoryMock.GetPaginatedByCategoryAsync(category.Id, query.PageNumber, query.PageSize, Arg.Any<CancellationToken>())
            .Returns(paginatedItems);

        // Act
        var result = await ListItemsByCategoryHandler.Handle(
            query,
            _categoryRepositoryMock,
            _itemRepositoryMock,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var paginatedDto = result.Value;
        paginatedDto.Should().NotBeNull();
        paginatedDto.TotalCount.Should().Be(1);
        paginatedDto.PageNumber.Should().Be(1);
        paginatedDto.Items.Should().HaveCount(1);

        var firstDto = paginatedDto.Items.GetEnumerator();
        firstDto.MoveNext();
        var dto = firstDto.Current;

        dto.Id.Should().Be(item.Id);
        dto.Name.Should().Be("The Witcher 3");
        dto.Attributes.Should().ContainKey("Plataforma");
        dto.Attributes["Plataforma"].Should().Be("PC");
    }
}
