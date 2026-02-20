using FluentAssertions;
using Valora.Application.UseCases.Categories.Create;
using Valora.Domain.Entities;
using Valora.UnitTests.Abstractions;

namespace Valora.UnitTests.Application.UseCases.Categories.Create;

/// <summary>
/// Testes unitários para as regras de validação de criação de categoria.
/// </summary>
public class CreateCategoryValidatorTests
{
    private readonly CreateCategoryValidator _validator = new();

    [Fact(DisplayName = "Deve ser válido quando todos os dados estiverem corretos")]
    public void Validate_Should_BeValid_WhenCommandIsCorrect()
    {
        // Arrange
        var command = new CreateCategoryCommand(
            Constants.Category.Name,
            Constants.Category.Description,
            new List<CategoryFieldDto>
            {
                new("Título", FieldType.Text, true),
                new("Data de Lançamento", FieldType.Date, false)
            }
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory(DisplayName = "Deve apontar erro quando o Nome for nulo, vazio ou conter apenas espaços")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_Should_HaveError_WhenNameIsInvalid(string? invalidName)
    {
        // Arrange
        var command = new CreateCategoryCommand(invalidName!, Constants.Category.Description);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name" 
                                             && e.ErrorMessage.Contains("obrigatório"));
    }

    [Fact(DisplayName = "Deve apontar erro quando o Nome exceder 50 caracteres")]
    public void Validate_Should_HaveError_WhenNameExceedsMaxLength()
    {
        // Arrange (DRY: Usando o construtor de string para criar 51 caracteres rapidamente)
        var longName = new string('A', 51);
        var command = new CreateCategoryCommand(longName, Constants.Category.Description);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("50 caracteres"));
    }

    [Fact(DisplayName = "Deve apontar erro quando a Descrição exceder 200 caracteres")]
    public void Validate_Should_HaveError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var longDescription = new string('A', 201);
        var command = new CreateCategoryCommand(Constants.Category.Name, longDescription);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Description");
    }

    [Fact(DisplayName = "Deve apontar erro no Schema quando o nome do campo for vazio")]
    public void Validate_Should_HaveError_WhenSchemaFieldNameIsEmpty()
    {
        // Arrange
        var schema = new List<CategoryFieldDto> { new("", FieldType.Text, false) };
        var command = new CreateCategoryCommand(Constants.Category.Name, Constants.Category.Description, schema);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Name") 
                                            && e.PropertyName.StartsWith("Schema"));
    }

    [Fact(DisplayName = "Deve apontar erro no Schema quando o tipo de campo não existir no Enum")]
    public void Validate_Should_HaveError_WhenSchemaFieldTypeIsInvalid()
    {
        // Arrange
        var invalidType = (FieldType)999; 
        var schema = new List<CategoryFieldDto> { new("Campo Inválido", invalidType, false) };
        var command = new CreateCategoryCommand(Constants.Category.Name, Constants.Category.Description, schema);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Type") 
                                            && e.PropertyName.StartsWith("Schema"));
    }
}