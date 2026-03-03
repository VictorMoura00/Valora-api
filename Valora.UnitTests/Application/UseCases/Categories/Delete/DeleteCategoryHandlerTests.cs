using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Valora.Application.UseCases.Categories.Delete;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;
using Xunit;

namespace Valora.UnitTests.Application.UseCases.Categories.Delete;

public class DeleteCategoryHandlerTests
{
    private readonly ICategoryRepository _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();

    [Fact(DisplayName = "Deve retornar falha (NotFound) quando a categoria não existir")]
    public async Task Handle_Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var command = new DeleteCategoryCommand(Guid.NewGuid());

        _categoryRepositoryMock.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        // Act
        var result = await DeleteCategoryHandler.Handle(
            command,
            _categoryRepositoryMock,
            _unitOfWorkMock,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Category.NotFound");

        await _categoryRepositoryMock.DidNotReceiveWithAnyArgs().DeleteAsync(default, default);
        await _unitOfWorkMock.DidNotReceiveWithAnyArgs().CommitAsync(default);
    }

    [Fact(DisplayName = "Deve deletar e retornar sucesso quando a categoria for encontrada")]
    public async Task Handle_Should_DeleteAndReturnSuccess_WhenCategoryExists()
    {
        // Arrange
        var command = new DeleteCategoryCommand(Guid.NewGuid());
        var category = new Category("Tecnologia", "Itens de tecnologia");

        _categoryRepositoryMock.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(category);

        // Act
        var result = await DeleteCategoryHandler.Handle(
            command,
            _categoryRepositoryMock,
            _unitOfWorkMock,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _categoryRepositoryMock.Received(1).DeleteAsync(category.Id, Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
