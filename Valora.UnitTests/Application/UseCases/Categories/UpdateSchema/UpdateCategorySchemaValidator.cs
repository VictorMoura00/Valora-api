using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Valora.Application.UseCases.Categories.UpdateSchema;
using Valora.Domain.Entities;

namespace Valora.UnitTests.Application.UseCases.Categories.UpdateSchema;

public class UpdateCategorySchemaValidatorTests
{
    private readonly UpdateCategorySchemaValidator _validator = new();

    [Fact(DisplayName = "Deve ser válido quando todos os dados do comando estiverem corretos")]
    public void Validate_Should_BeValid_WhenCommandIsCorrect()
    {
        // Arrange
        var command = new UpdateCategorySchemaCommand(
            Guid.NewGuid(),
            new List<CategoryFieldDto>
            {
                new("Cor", FieldType.Text, true),
                new("Ano", FieldType.Number, false)
            }
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Deve apontar erro quando o Id da categoria for vazio")]
    public void Validate_Should_HaveError_WhenIdIsEmpty()
    {
        // Arrange
        var command = new UpdateCategorySchemaCommand(Guid.Empty, new List<CategoryFieldDto>());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == "Id" &&
            e.ErrorMessage.Contains("obrigatório"));
    }

    [Fact(DisplayName = "Deve apontar erro quando a lista de Schema for nula")]
    public void Validate_Should_HaveError_WhenSchemaIsNull()
    {
        // Arrange
        var command = new UpdateCategorySchemaCommand(Guid.NewGuid(), null!);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.PropertyName == "Schema" &&
            e.ErrorMessage.Contains("nula"));
    }

    [Theory(DisplayName = "Deve apontar erro quando o nome de algum campo do schema for vazio ou nulo")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_Should_HaveError_WhenSchemaFieldNameIsInvalid(string? invalidName)
    {
        // Arrange
        var schema = new List<CategoryFieldDto> { new(invalidName!, FieldType.Text, false) };
        var command = new UpdateCategorySchemaCommand(Guid.NewGuid(), schema);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName.StartsWith("Schema") &&
            e.PropertyName.Contains("Name") &&
            e.ErrorMessage.Contains("obrigatório"));
    }

    [Fact(DisplayName = "Deve apontar erro quando o nome de algum campo do schema exceder 50 caracteres")]
    public void Validate_Should_HaveError_WhenSchemaFieldNameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('A', 51);
        var schema = new List<CategoryFieldDto> { new(longName, FieldType.Text, false) };
        var command = new UpdateCategorySchemaCommand(Guid.NewGuid(), schema);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e =>
            e.PropertyName.StartsWith("Schema") &&
            e.PropertyName.Contains("Name") &&
            e.ErrorMessage.Contains("50 caracteres"));
    }

    [Fact(DisplayName = "Deve apontar erro quando o tipo de algum campo do schema não existir no Enum")]
    public void Validate_Should_HaveError_WhenSchemaFieldTypeIsInvalid()
    {
        // Arrange
        var invalidType = (FieldType)999;
        var schema = new List<CategoryFieldDto> { new("Campo Inválido", invalidType, false) };
        var command = new UpdateCategorySchemaCommand(Guid.NewGuid(), schema);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName.StartsWith("Schema") &&
            e.PropertyName.Contains("Type") &&
            e.ErrorMessage.Contains("inválido"));
    }
}