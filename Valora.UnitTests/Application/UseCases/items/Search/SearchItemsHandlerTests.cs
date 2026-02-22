using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Valora.Application.UseCases.Items.Search;
using Valora.Domain.Common.Pagination;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;
using Xunit;

namespace Valora.UnitTests.Application.UseCases.Items.Search;

public class SearchItemsHandlerTests
{
    private readonly IItemRepository _itemRepositoryMock = Substitute.For<IItemRepository>();

    [Fact(DisplayName = "Deve retornar sucesso e uma lista vazia quando nenhum item corresponder à busca")]
    public async Task Handle_Should_ReturnSuccessWithEmptyList_WhenNoItemsMatch()
    {
        // Arrange
        var query = new SearchItemsQuery("TermoInexistente", PageNumber: 1, PageSize: 10);

        var emptyPagination = new PaginatedList<Item>(
            items: new List<Item>(),
            count: 0,
            pageNumber: 1,
            pageSize: 10);

        _itemRepositoryMock.SearchByNameAsync(
            query.SearchTerm, query.PageNumber, query.PageSize, Arg.Any<CancellationToken>())
            .Returns(emptyPagination);

        // Act
        var result = await SearchItemsHandler.Handle(query, _itemRepositoryMock, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.TotalCount.Should().Be(0);
        result.Value.Items.Should().BeEmpty();
    }

    [Fact(DisplayName = "Deve retornar sucesso e mapear os DTOs corretamente quando encontrar itens")]
    public async Task Handle_Should_ReturnSuccessAndMappedDtos_WhenItemsAreFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var item = new Item(categoryId, "iPhone 15 Pro");
        item.SetAttribute("Cor", "Titânio");

        var paginatedItems = new PaginatedList<Item>(
            items: new List<Item> { item },
            count: 1,
            pageNumber: 1,
            pageSize: 10);

        var query = new SearchItemsQuery("iphone", PageNumber: 1, PageSize: 10);

        _itemRepositoryMock.SearchByNameAsync(
            query.SearchTerm, query.PageNumber, query.PageSize, Arg.Any<CancellationToken>())
            .Returns(paginatedItems);

        // Act
        var result = await SearchItemsHandler.Handle(query, _itemRepositoryMock, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var dtoList = result.Value;
        dtoList.TotalCount.Should().Be(1);
        dtoList.Items.Should().HaveCount(1);

        var firstDto = dtoList.Items.GetEnumerator();
        firstDto.MoveNext();
        var dto = firstDto.Current;

        dto.Id.Should().Be(item.Id);
        dto.CategoryId.Should().Be(categoryId);
        dto.Name.Should().Be("iPhone 15 Pro");
        dto.Attributes.Should().ContainKey("Cor");
        dto.Attributes["Cor"].Should().Be("Titânio");
    }
}
