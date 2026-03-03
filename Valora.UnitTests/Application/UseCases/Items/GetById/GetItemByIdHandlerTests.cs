using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Valora.Application.UseCases.Items.GetById;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;
using Xunit;

namespace Valora.UnitTests.Application.UseCases.Items.GetById;

public class GetItemByIdHandlerTests
{
    private readonly IItemRepository _itemRepositoryMock = Substitute.For<IItemRepository>();

    [Fact(DisplayName = "Deve retornar falha (NotFound) quando o item não existir no banco")]
    public async Task Handle_Should_ReturnFailure_WhenItemDoesNotExist()
    {
        // Arrange
        var query = new GetItemByIdQuery(Guid.NewGuid());

        _itemRepositoryMock.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns((Item?)null);

        // Act
        var result = await GetItemByIdHandler.Handle(
            query,
            _itemRepositoryMock,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Item.NotFound");
    }

    [Fact(DisplayName = "Deve retornar sucesso e o DTO mapeado corretamente quando o item for encontrado")]
    public async Task Handle_Should_ReturnSuccessAndMappedDto_WhenItemExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var item = new Item(categoryId, "MacBook Pro M3");

        item.SetAttribute("MemoriaRAM", "16GB");
        item.SetAttribute("Armazenamento", "512GB");

        var query = new GetItemByIdQuery(item.Id);

        _itemRepositoryMock.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns(item);

        // Act
        var result = await GetItemByIdHandler.Handle(
            query,
            _itemRepositoryMock,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var dto = result.Value;
        dto.Should().NotBeNull();
        dto.Id.Should().Be(item.Id);
        dto.CategoryId.Should().Be(item.CategoryId);
        dto.Name.Should().Be("MacBook Pro M3");

        dto.Attributes.Should().HaveCount(2);
        dto.Attributes["MemoriaRAM"].Should().Be("16GB");
        dto.Attributes["Armazenamento"].Should().Be("512GB");

        dto.CreatedAt.Should().Be(item.CreatedAt);
        dto.UpdatedAt.Should().Be(item.UpdatedAt);
    }
}
