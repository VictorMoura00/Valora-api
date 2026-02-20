using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Valora.Domain.Common.Results;

namespace Valora.UnitTests.Domain.Common.Results;

public class ResultTTests
{
    [Fact(DisplayName = "Acessar Value de um Result com falha deve lançar InvalidOperationException")]
    public void Value_Should_ThrowException_WhenResultIsFailure()
    {
        // Arrange
        var result = Result<string>.Failure(Error.NotFound("Code", "Desc"));

        // Act & Assert
        var action = () => result.Value;
        action.Should().Throw<InvalidOperationException>()
              .WithMessage("*Não é possível acessar o valor*");
    }

    [Fact(DisplayName = "Operador Implícito deve converter valor válido para Success")]
    public void ImplicitOperator_Should_ConvertValidValue_ToSuccess()
    {
        // Arrange
        string valorQualquer = "Testando conversão";

        // Act
        Result<string> result = valorQualquer;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(valorQualquer);
    }

    [Fact(DisplayName = "Operador Implícito deve converter valor nulo para Falha com Error.NullValue")]
    public void ImplicitOperator_Should_ConvertNull_ToFailureWithNullError()
    {
        // Arrange
        string? valorNulo = null;

        // Act
        Result<string> result = valorNulo;

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }

    [Fact(DisplayName = "Operador Implícito deve converter Error para Result de Falha")]
    public void ImplicitOperator_Should_ConvertError_ToFailure()
    {
        // Arrange
        var error = Error.Conflict("Conflito", "Algo deu errado");

        // Act
        Result<int> result = error;

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact(DisplayName = "Match deve executar a função de Sucesso quando IsSuccess for verdadeiro")]
    public void Match_Should_ExecuteSuccessFunction_WhenIsSuccess()
    {
        // Arrange
        Result<int> result = 10;

        // Act
        var matchResult = result.Match(
            onSuccess: val => $"Sucesso: {val}",
            onFailure: err => "Falhou"
        );

        // Assert
        matchResult.Should().Be("Sucesso: 10");
    }

    [Fact(DisplayName = "Match deve executar a função de Falha quando IsFailure for verdadeiro")]
    public void Match_Should_ExecuteFailureFunction_WhenIsFailure()
    {
        // Arrange
        Result<int> result = Error.NotFound("Code", "Desc");

        // Act
        var matchResult = result.Match(
            onSuccess: val => "Sucesso",
            onFailure: err => $"Falha: {err.Code}"
        );

        // Assert
        matchResult.Should().Be("Falha: Code");
    }
}
