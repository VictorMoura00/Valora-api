using System;
using FluentAssertions;
using Xunit;
using Valora.Domain.Common.Results;

namespace Valora.UnitTests.Domain.Common.Results;

/// <summary>
/// Testes de unidade para os blocos de construção de Result e Error.
/// Autor: Victor Moura
/// </summary>
public class ErrorTests
{
    [Fact(DisplayName = "Deve criar um erro com o tipo correto para cada método Factory")]
    public void FactoryMethods_Should_CreateErrors_WithCorrectTypes()
    {
        // Act
        var validationError = Error.Validation("Val.Code", "Descrição");
        var notFoundError = Error.NotFound("NF.Code", "Descrição");
        var conflictError = Error.Conflict("Conf.Code", "Descrição");
        var failureError = Error.Failure("Fail.Code", "Descrição");

        // Assert
        validationError.Type.Should().Be(ErrorType.Validation);
        notFoundError.Type.Should().Be(ErrorType.NotFound);
        conflictError.Type.Should().Be(ErrorType.Conflict);
        failureError.Type.Should().Be(ErrorType.Failure);
    }

    [Fact(DisplayName = "Error.None deve ter estado nulo e tipo None")]
    public void None_Should_HaveEmptyState_And_NoneType()
    {
        // Assert
        Error.None.Code.Should().BeEmpty();
        Error.None.Description.Should().BeEmpty();
        Error.None.Type.Should().Be(ErrorType.None);
    }
}
