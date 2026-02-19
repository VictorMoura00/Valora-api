using FluentAssertions;
using NSubstitute;
using Valora.Application.UseCases.Items.Create;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;
using Valora.UnitTests.Abstractions; 
using Xunit;

namespace Valora.UnitTests.Application.UseCases.Items.Create;

public class CreateItemHandlerTests
{
    private readonly IItemRepository _itemRepoMock;
    private readonly ICategoryRepository _categoryRepoMock;
    private readonly IUnitOfWork _uowMock;
    private readonly CreateItemHandler _handler;

    public CreateItemHandlerTests()
    {
        _itemRepoMock = Substitute.For<IItemRepository>();
        _categoryRepoMock = Substitute.For<ICategoryRepository>();
        _uowMock = Substitute.For<IUnitOfWork>();
        _handler = new CreateItemHandler(_itemRepoMock, _categoryRepoMock, _uowMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CategoryNotFound()
    {
        // Arrange
        var command = new CreateItemCommand(Constants.Category.Id, new Dictionary<string, object>());
        
        // Mockando retorno nulo
        _categoryRepoMock.GetByIdAsync(Constants.Category.Id).Returns((Category?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Usando nossa extensão personalizada!)
        result.Should().BeFailure(Error.NotFound("Category.NotFound", ""));
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Valid()
    {
        // Arrange
        var category = new Category(Constants.Category.Name, Constants.Category.Description);
        // Configurando schema simples
        category.AddField("Titulo", FieldType.Text, required: true);
        
        var fields = new Dictionary<string, object> { { "Titulo", Constants.Item.ValidTitle } };
        var command = new CreateItemCommand(category.Id, fields);

        _categoryRepoMock.GetByIdAsync(category.Id).Returns(category);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
        
        // Verifica se chamou o commit
        await _uowMock.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}