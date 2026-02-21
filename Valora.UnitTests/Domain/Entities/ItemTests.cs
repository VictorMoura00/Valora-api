using System;
using System.Collections.Generic;
using FluentAssertions;
using Valora.Domain.Entities;
using Xunit;

namespace Valora.UnitTests.Domain.Entities;

public class ItemTests
{
    private readonly Guid _validCategoryId = Guid.NewGuid();
    private const string ValidName = "iPhone 15 Pro";

    [Fact(DisplayName = "Construtor deve instanciar Item corretamente com dados válidos")]
    public void Constructor_Should_CreateItem_WhenValidData()
    {
        // Act
        var item = new Item(_validCategoryId, ValidName);

        // Assert
        item.Id.Should().NotBeEmpty();
        item.CategoryId.Should().Be(_validCategoryId);
        item.Name.Should().Be(ValidName);
        item.Attributes.Should().BeEmpty();
        item.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact(DisplayName = "Construtor deve lançar exceção se CategoryId for vazio")]
    public void Constructor_Should_ThrowException_WhenCategoryIdIsEmpty()
    {
        // Act
        Action action = () => new Item(Guid.Empty, ValidName);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*categoria é obrigatória*");
    }

    [Theory(DisplayName = "Construtor deve lançar exceção se o Nome for nulo ou vazio")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_Should_ThrowException_WhenNameIsInvalid(string invalidName)
    {
        // Act
        Action action = () => new Item(_validCategoryId, invalidName);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact(DisplayName = "UpdateName deve atualizar o nome e preencher UpdatedAt quando válido")]
    public void UpdateName_Should_UpdateName_And_SetUpdated_WhenValid()
    {
        // Arrange
        var item = new Item(_validCategoryId, ValidName);
        var newName = "iPhone 16 Pro";

        // Act
        var result = item.UpdateName(newName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        item.Name.Should().Be(newName);
        item.UpdatedAt.Should().NotBeNull();
    }

    [Fact(DisplayName = "UpdateName deve retornar falha se o nome for inválido")]
    public void UpdateName_Should_ReturnFailure_WhenNameIsInvalid()
    {
        // Arrange
        var item = new Item(_validCategoryId, ValidName);

        // Act
        var result = item.UpdateName("   ");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Item.InvalidName");
        item.Name.Should().Be(ValidName); // Não deve ter alterado
    }

    [Fact(DisplayName = "SetAttribute deve adicionar um novo atributo se não existir")]
    public void SetAttribute_Should_AddNewAttribute()
    {
        // Arrange
        var item = new Item(_validCategoryId, ValidName);

        // Act
        item.SetAttribute("Marca", "Apple");
        item.SetAttribute("Armazenamento", 256);

        // Assert
        item.Attributes.Should().HaveCount(2);
        item.Attributes["Marca"].Should().Be("Apple");
        item.Attributes["Armazenamento"].Should().Be(256);
        item.UpdatedAt.Should().NotBeNull();
    }

    [Fact(DisplayName = "SetAttribute deve remover o atributo se o valor for nulo ou string vazia")]
    public void SetAttribute_Should_RemoveAttribute_WhenValueIsNullOrEmpty()
    {
        // Arrange
        var item = new Item(_validCategoryId, ValidName);
        item.SetAttribute("Cor", "Preto"); // Adiciona primeiro

        // Act - Passando null
        item.SetAttribute("Cor", null!);

        // Assert
        item.Attributes.Should().NotContainKey("Cor");

        // Act - Passando string vazia
        item.SetAttribute("Marca", "Apple");
        item.SetAttribute("Marca", "   ");

        // Assert
        item.Attributes.Should().NotContainKey("Marca");
    }

    [Fact(DisplayName = "ReplaceAttributes deve substituir todo o dicionário de atributos")]
    public void ReplaceAttributes_Should_ReplaceAllAttributes()
    {
        // Arrange
        var item = new Item(_validCategoryId, ValidName);
        item.SetAttribute("Velho", "Valor");

        var novosAtributos = new Dictionary<string, object>
        {
            { "Novo1", "A" },
            { "Novo2", 123 }
        };

        // Act
        item.ReplaceAttributes(novosAtributos);

        // Assert
        item.Attributes.Should().HaveCount(2);
        item.Attributes.Should().NotContainKey("Velho");
        item.Attributes["Novo1"].Should().Be("A");
    }
}
