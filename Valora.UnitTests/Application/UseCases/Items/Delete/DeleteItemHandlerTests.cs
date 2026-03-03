using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Valora.Application.UseCases.Items.Delete;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;
using Xunit;

namespace Valora.UnitTests.Application.UseCases.Items.Delete;

public class DeleteItemHandlerTests
{
    private readonly IItemRepository _itemRepositoryMock = Substitute.For<IItemRepository>();
    private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();

    [Fact(DisplayName = "Deve retornar falha (NotFound) quando o item não existir")]
    public async Task Handle_Should_ReturnFailure_WhenItemDoesNotExist()
    {
        // Arrange
        var command = new DeleteItemCommand(Guid.NewGuid());

        _itemRepositoryMock.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Item?)null);

        // Act
        var result = await DeleteItemHandler.Handle(
            command,
            _itemRepositoryMock,
            _unitOfWorkMock,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Item.NotFound");

        await _itemRepositoryMock.DidNotReceiveWithAnyArgs().DeleteAsync(default, default);
        await _unitOfWorkMock.DidNotReceiveWithAnyArgs().CommitAsync(default);
    }

    [Fact(DisplayName = "Deve deletar de forma lógica e retornar sucesso quando o item existir")]
    public async Task Handle_Should_SoftDeleteAndReturnSuccess_WhenItemExists()
    {
        // Arrange
        var command = new DeleteItemCommand(Guid.NewGuid());
        var item = new Item(Guid.NewGuid(), "PlayStation 5");

        _itemRepositoryMock.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(item);

        // Act
        var result = await DeleteItemHandler.Handle(
            command,
            _itemRepositoryMock,
            _unitOfWorkMock,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _itemRepositoryMock.Received(1).DeleteAsync(item.Id, Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
