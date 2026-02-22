using FluentAssertions;
using NSubstitute;
using Valora.Application.UseCases.Items.Update;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.UnitTests.Application.UseCases.Items.Update;

public class UpdateItemHandlerTests
{
    private readonly ICategoryRepository _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
    private readonly IItemRepository _itemRepositoryMock = Substitute.For<IItemRepository>();
    private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();

    [Fact(DisplayName = "Deve retornar falha (NotFound) se o item não for encontrado")]
    public async Task Handle_Should_ReturnFailure_WhenItemDoesNotExist()
    {
        // Arrange
        var command = new UpdateItemCommand(Guid.NewGuid(), "Novo Nome", new Dictionary<string, object>());

        _itemRepositoryMock.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Item?)null);

        // Act
        var result = await UpdateItemHandler.Handle(
            command, _categoryRepositoryMock, _itemRepositoryMock, _unitOfWorkMock, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Item.NotFound");

        await _itemRepositoryMock.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }

    [Fact(DisplayName = "Deve retornar falha (Conflict) se alterar o nome para um que já existe na categoria")]
    public async Task Handle_Should_ReturnFailure_WhenChangingNameToAnExistingOne()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var itemExistente = new Item(categoryId, "Nome Antigo");
        var command = new UpdateItemCommand(itemExistente.Id, "Nome Novo", new Dictionary<string, object>());

        _itemRepositoryMock.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(itemExistente);
        _itemRepositoryMock.ExistsByNameAsync(command.Name, categoryId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await UpdateItemHandler.Handle(
            command, _categoryRepositoryMock, _itemRepositoryMock, _unitOfWorkMock, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Item.NameAlreadyExists");
    }

    [Fact(DisplayName = "Deve retornar falha de validação se não respeitar o Schema da Categoria")]
    public async Task Handle_Should_ReturnFailure_WhenSchemaValidationFails()
    {
        // Arrange
        var category = new Category("Filmes", "Catálogo");
        category.AddField("Diretor", FieldType.Text, required: true);

        var item = new Item(category.Id, "Inception");

        var command = new UpdateItemCommand(item.Id, "Inception", new Dictionary<string, object>());

        _itemRepositoryMock.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        _categoryRepositoryMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        // Act
        var result = await UpdateItemHandler.Handle(
            command, _categoryRepositoryMock, _itemRepositoryMock, _unitOfWorkMock, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Item.MissingRequiredAttribute");

        await _itemRepositoryMock.DidNotReceiveWithAnyArgs().ExistsByNameAsync(default!, default);
    }

    [Fact(DisplayName = "Deve atualizar com sucesso quando manter o mesmo nome e enviar atributos válidos")]
    public async Task Handle_Should_UpdateAndReturnSuccess_WhenKeepingSameName_And_ValidSchema()
    {
        // Arrange
        var category = new Category("Filmes", "Catálogo");
        category.AddField("Diretor", FieldType.Text, required: true);

        var item = new Item(category.Id, "Inception");
        item.SetAttribute("Diretor", "Desconhecido");

        var validAttributes = new Dictionary<string, object> { { "Diretor", "Christopher Nolan" } };
        var command = new UpdateItemCommand(item.Id, "Inception", validAttributes);

        _itemRepositoryMock.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);
        _categoryRepositoryMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        // Act
        var result = await UpdateItemHandler.Handle(
            command, _categoryRepositoryMock, _itemRepositoryMock, _unitOfWorkMock, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _itemRepositoryMock.DidNotReceiveWithAnyArgs().ExistsByNameAsync(default!, default);

        await _itemRepositoryMock.Received(1).UpdateAsync(
            Arg.Is<Item>(i => i.Attributes["Diretor"].ToString() == "Christopher Nolan"),
            Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
