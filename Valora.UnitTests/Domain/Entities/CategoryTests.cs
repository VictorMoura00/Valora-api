using System;
using System.Linq;
using Xunit;
using Valora.Domain.Entities;
using Valora.Domain.Common.Results;

namespace Valora.UnitTests.Domain.Entities;

public class CategoryTests
{
    [Fact]
    public void Construtor_DeveCriarCategoria_QuandoDadosForemValidos()
    {
        // Arrange & Act
        var category = new Category("Tecnologia", "Equipamentos de TI");

        // Assert
        Assert.Equal("Tecnologia", category.Name);
        Assert.Equal("Equipamentos de TI", category.Description);

        Assert.Null(category.UpdatedAt);
        Assert.Empty(category.Schema);
    }

    [Theory]
    [InlineData("", "Descrição válida")]
    [InlineData("Nome válido", null)]
    [InlineData("   ", "   ")]
    public void Construtor_DeveLancarExcecao_QuandoDadosForemInvalidos(string name, string description)
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new Category(name, description));
    }

    [Fact]
    public void Update_DeveRetornarSucesso_AlterarDados_E_AtualizarData()
    {
        // Arrange
        var category = new Category("Nome Antigo", "Descrição Antiga");
        var dataCriacao = category.CreatedAt;

        // Act
        var result = category.Update("Nome Novo", "Descrição Nova");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Nome Novo", category.Name);
        Assert.Equal("Descrição Nova", category.Description);
        Assert.NotNull(category.UpdatedAt);
        Assert.True(category.UpdatedAt >= dataCriacao);
    }

    [Theory]
    [InlineData("", "Descrição válida", "Category.InvalidName")]
    [InlineData("Nome válido", "", "Category.InvalidDescription")]
    public void Update_DeveRetornarFalha_QuandoDadosForemInvalidos(string name, string description, string expectedErrorCode)
    {
        // Arrange
        var category = new Category("Nome Antigo", "Descrição Antiga");

        // Act
        var result = category.Update(name, description);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(expectedErrorCode, result.Error.Code);
        Assert.Equal("Nome Antigo", category.Name);
    }

    [Fact]
    public void AddField_DeveRetornarSucesso_AdicionarCampoNoSchema_E_AtualizarData()
    {
        // Arrange
        var category = new Category("Imóveis", "Categoria de imóveis");

        // Act
        var result = category.AddField("Metragem", FieldType.Number, true);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(category.Schema);

        var field = category.Schema.First();
        Assert.Equal("Metragem", field.Name);
        Assert.Equal(FieldType.Number, field.Type);
        Assert.True(field.IsRequired);
        Assert.NotNull(category.UpdatedAt);
    }

    [Fact]
    public void AddField_DeveRetornarFalha_QuandoCampoJaExistir()
    {
        // Arrange
        var category = new Category("Veículos", "Categoria de veículos");
        category.AddField("Placa", FieldType.Text, true);

        // Act 
        var result = category.AddField("placa", FieldType.Text, false);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Category.DuplicateField", result.Error.Code);
        Assert.Single(category.Schema);
    }

    [Fact]
    public void AddField_DeveRetornarFalha_QuandoNomeDoCampoForVazio()
    {
        // Arrange
        var category = new Category("Veículos", "Categoria de veículos");

        // Act
        var result = category.AddField("   ", FieldType.Text, true);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Category.InvalidFieldName", result.Error.Code);
        Assert.Empty(category.Schema);
    }
}