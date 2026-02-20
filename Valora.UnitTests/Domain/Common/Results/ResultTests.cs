using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Valora.Domain.Common.Results;

namespace Valora.UnitTests.Domain.Common.Results;

public class ResultTests
{
    [Fact(DisplayName = "Success() deve retornar IsSuccess verdadeiro e Error.None")]
    public void Success_Should_ReturnIsSuccessTrue_And_ErrorNone()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact(DisplayName = "Failure() deve retornar IsSuccess falso e conter o Erro")]
    public void Failure_Should_ReturnIsFailureTrue_And_ContainError()
    {
        // Arrange
        var error = Error.Validation("Code", "Error Description");

        // Act
        var result = Result.Failure(error);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }
}
