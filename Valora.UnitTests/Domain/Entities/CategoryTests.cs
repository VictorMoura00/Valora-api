using System;
using System.Linq;
using Xunit;
using Valora.Domain.Entities;

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
        Assert.NotNull(category.UpdatedAt);
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
    public void Update_DeveAlterarDados_E_AtualizarData()
    {
        // Arrange
        var category = new Category("Nome Antigo", "Descrição Antiga");
        var dataAtualizacaoInicial = category.UpdatedAt;

        // Act
        category.Update("Nome Novo", "Descrição Nova");

        // Assert
        Assert.Equal("Nome Novo", category.Name);
        Assert.Equal("Descrição Nova", category.Description);
        Assert.True(category.UpdatedAt >= dataAtualizacaoInicial);
    }

    [Fact]
    public void AddField_DeveAdicionarCampoNoSchema_E_AtualizarData()
    {
        // Arrange
        var category = new Category("Imóveis", "Categoria de imóveis");

        // Act
        category.AddField("Metragem", FieldType.Number, true);

        // Assert
        Assert.Single(category.Schema);
        var field = category.Schema.First();
        Assert.Equal("Metragem", field.Name);
        Assert.Equal(FieldType.Number, field.Type);
        Assert.True(field.IsRequired);
    }

    [Fact]
    public void AddField_DeveLancarExcecao_QuandoCampoJaExistir()
    {
        // Arrange
        var category = new Category("Veículos", "Categoria de veículos");
        category.AddField("Placa", FieldType.Text, true);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            category.AddField("placa", FieldType.Text, false));

        Assert.Equal("O campo 'placa' já existe no schema desta categoria.", exception.Message);
    }
}