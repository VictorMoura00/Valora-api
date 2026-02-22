using FluentAssertions;
using NSubstitute;
using Valora.Application.UseCases.Items.Create;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.UnitTests.Application.UseCases.Items.Create;

public class CreateItemHandlerTests
{
    private readonly ICategoryRepository _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
    private readonly IItemRepository _itemRepositoryMock = Substitute.For<IItemRepository>();
    private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();

    [Fact(DisplayName = "Deve retornar falha (NotFound) se a categoria não existir")]
    public async Task Handle_Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var command = new CreateItemCommand(Guid.NewGuid(), "Teste", new Dictionary<string, object>());

        _categoryRepositoryMock.GetByIdAsync(command.CategoryId, Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        // Act
        var result = await CreateItemHandler.Handle(
            command, _categoryRepositoryMock, _itemRepositoryMock, _unitOfWorkMock, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Item.CategoryNotFound");

        await _itemRepositoryMock.DidNotReceiveWithAnyArgs().AddAsync(default!);
    }

    [Fact(DisplayName = "Deve retornar falha (Conflict) se o nome do item já existir na categoria")]
    public async Task Handle_Should_ReturnFailure_WhenItemNameAlreadyExists()
    {
        // Arrange
        var category = new Category("Filmes", "Catálogo de filmes");
        var command = new CreateItemCommand(category.Id, "O Poderoso Chefão", new Dictionary<string, object>());

        _categoryRepositoryMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
        _itemRepositoryMock.ExistsByNameAsync(command.Name, category.Id, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await CreateItemHandler.Handle(
            command, _categoryRepositoryMock, _itemRepositoryMock, _unitOfWorkMock, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Item.NameAlreadyExists");

        await _itemRepositoryMock.DidNotReceiveWithAnyArgs().AddAsync(default!);
    }

    [Fact(DisplayName = "Deve retornar falha de validação se faltar um atributo obrigatório do Schema")]
    public async Task Handle_Should_ReturnFailure_WhenMissingRequiredAttribute()
    {
        // Arrange
        var category = new Category("Filmes", "Catálogo");
        category.AddField("Diretor", FieldType.Text, required: true); // Campo obrigatório!

        // Enviando um dicionário vazio
        var command = new CreateItemCommand(category.Id, "Inception", new Dictionary<string, object>());

        _categoryRepositoryMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
        _itemRepositoryMock.ExistsByNameAsync(command.Name, category.Id, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await CreateItemHandler.Handle(
            command, _categoryRepositoryMock, _itemRepositoryMock, _unitOfWorkMock, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Item.MissingRequiredAttribute");
        result.Error.Description.Should().Contain("Diretor");
    }

    [Fact(DisplayName = "Deve retornar falha de validação se atributo obrigatório for enviado em branco")]
    public async Task Handle_Should_ReturnFailure_WhenRequiredAttributeIsEmptyString()
    {
        // Arrange
        var category = new Category("Filmes", "Catálogo");
        category.AddField("Diretor", FieldType.Text, required: true);

        var invalidAttributes = new Dictionary<string, object> { { "Diretor", "   " } };
        var command = new CreateItemCommand(category.Id, "Inception", invalidAttributes);

        _categoryRepositoryMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
        _itemRepositoryMock.ExistsByNameAsync(command.Name, category.Id, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await CreateItemHandler.Handle(
            command, _categoryRepositoryMock, _itemRepositoryMock, _unitOfWorkMock, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Item.EmptyRequiredAttribute");
    }

    [Fact(DisplayName = "Deve salvar o item e commitar quando todos os dados e o Schema forem válidos")]
    public async Task Handle_Should_SaveAndReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var category = new Category("Filmes", "Catálogo");
        category.AddField("Diretor", FieldType.Text, required: true);
        category.AddField("Ano", FieldType.Number, required: false);

        var validAttributes = new Dictionary<string, object>
        {
            { "Diretor", "Christopher Nolan" }, // Obrigatório preenchido
            { "Ano", 2010 }                     // Opcional preenchido
        };

        var command = new CreateItemCommand(category.Id, "Inception", validAttributes);

        _categoryRepositoryMock.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);
        _itemRepositoryMock.ExistsByNameAsync(command.Name, category.Id, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await CreateItemHandler.Handle(
            command, _categoryRepositoryMock, _itemRepositoryMock, _unitOfWorkMock, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        await _itemRepositoryMock.Received(1).AddAsync(
            Arg.Is<Item>(i => i.Name == "Inception" &&
                              i.CategoryId == category.Id &&
                              i.Attributes.Count == 2),
            Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
