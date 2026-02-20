using FluentAssertions;
using NSubstitute;
using Valora.Application.UseCases.Categories.Create;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;
using Valora.UnitTests.Abstractions;

namespace Valora.UnitTests.Application.UseCases.Items.Create;

/// <summary>
/// Testes unitários para o caso de uso de criação de categoria.
/// Autor: Victor Moura
/// </summary>
public class CreateCategoryHandlerTests
{
    private readonly ICategoryRepository _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();

    [Fact(DisplayName = "Deve retornar falha (Conflict) quando o nome da categoria já existir")]
    public async Task Handle_Should_ReturnFailure_WhenNameAlreadyExists()
    {
        // Arrange
        var command = new CreateCategoryCommand(Constants.Category.Name, Constants.Category.Description);
        var existingCategory = new Category(Constants.Category.Name, "Outra descrição qualquer");
        
        _categoryRepositoryMock.GetByNameAsync(command.Name).Returns(existingCategory);

        var expectedError = Error.Conflict(
            "Category.DuplicateName",
            $"Já existe uma categoria com o nome '{command.Name}'."
        );

        // Act
        var result = await CreateCategoryHandler.Handle(
            command,
            _categoryRepositoryMock,
            _unitOfWorkMock,
            CancellationToken.None);

        // Assert
        result.Should().BeFailure(expectedError);

        await _categoryRepositoryMock.DidNotReceiveWithAnyArgs().AddAsync(default!);
        await _unitOfWorkMock.DidNotReceiveWithAnyArgs().CommitAsync(default);
    }

    [Fact(DisplayName = "Deve criar a categoria e retornar sucesso com o ID quando os dados forem válidos")]
    public async Task Handle_Should_ReturnSuccess_WhenCategoryIsNew()
    {
        // Arrange
        var command = new CreateCategoryCommand(Constants.Category.Name, Constants.Category.Description);

        _categoryRepositoryMock.GetByNameAsync(command.Name).Returns((Category?)null);

        // Act
        var result = await CreateCategoryHandler.Handle(
            command,
            _categoryRepositoryMock,
            _unitOfWorkMock,
            CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        result.Value.Should().NotBeEmpty();

        await _categoryRepositoryMock.Received(1).AddAsync(Arg.Is<Category>(c => 
            c.Name == command.Name && 
            c.Description == command.Description));

        await _unitOfWorkMock.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}