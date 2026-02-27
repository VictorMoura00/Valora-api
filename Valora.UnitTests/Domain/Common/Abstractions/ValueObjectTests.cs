using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Valora.UnitTests.Common.Stub.ValueObjectStubs;

namespace Valora.UnitTests.Domain.Common.Abstractions;

public class ValueObjectTests
{
    [Fact(DisplayName = "Equals deve retornar true quando os componentes forem iguais")]
    public void Equals_Should_ReturnTrue_WhenComponentsAreEqual()
    {
        // Arrange
        var point1 = new PointStub(1, 2);
        var point2 = new PointStub(1, 2);

        // Act & Assert
        point1.Equals(point2).Should().BeTrue();
        point1.Equals((object)point2).Should().BeTrue(); 
    }

    [Fact(DisplayName = "Equals deve retornar false quando os componentes forem diferentes")]
    public void Equals_Should_ReturnFalse_WhenComponentsAreDifferent()
    {
        // Arrange
        var point1 = new PointStub(1, 2);
        var point2 = new PointStub(2, 1);

        // Act & Assert
        point1.Equals(point2).Should().BeFalse();
    }

    [Fact(DisplayName = "Equals deve retornar false quando comparado com nulo ou tipo diferente")]
    public void Equals_Should_ReturnFalse_WhenComparedWithNullOrDifferentType()
    {
        // Arrange
        var point1 = new PointStub(1, 2);
        var point3D = new Point3DStub(1, 2, 0);

        // Act & Assert
        point1.Equals(null).Should().BeFalse();
        point1.Equals(point3D).Should().BeFalse();
    }

    [Fact(DisplayName = "Operador == deve retornar true quando os valores forem iguais")]
    public void EqualityOperator_Should_ReturnTrue_WhenValuesAreEqual()
    {
        // Arrange
        var point1 = new PointStub(1, 2);
        var point2 = new PointStub(1, 2);

        // Act & Assert
        (point1 == point2).Should().BeTrue();
    }

    [Fact(DisplayName = "Operador != deve retornar true quando os valores forem diferentes")]
    public void InequalityOperator_Should_ReturnTrue_WhenValuesAreDifferent()
    {
        // Arrange
        var point1 = new PointStub(1, 2);
        var point2 = new PointStub(9, 9);

        // Act & Assert
        (point1 != point2).Should().BeTrue();
    }

    [Fact(DisplayName = "Operadores devem lidar com instâncias nulas de forma segura")]
    public void Operators_Should_HandleNullsSafely()
    {
        // Arrange
        PointStub? point1 = new PointStub(1, 2);
        PointStub? pointNull1 = null;
        PointStub? pointNull2 = null;

        // Act & Assert
        (point1 == pointNull1).Should().BeFalse();
        (pointNull1 == point1).Should().BeFalse();
        (pointNull1 == pointNull2).Should().BeTrue(); 

        (point1 != pointNull1).Should().BeTrue();
    }

    [Fact(DisplayName = "GetHashCode deve retornar o mesmo valor para componentes iguais")]
    public void GetHashCode_Should_ReturnSameValue_ForEqualComponents()
    {
        // Arrange
        var point1 = new PointStub(5, 5);
        var point2 = new PointStub(5, 5);

        // Act & Assert
        point1.GetHashCode().Should().Be(point2.GetHashCode());
    }

    [Fact(DisplayName = "GetHashCode deve retornar valores diferentes para componentes diferentes")]
    public void GetHashCode_Should_ReturnDifferentValues_ForDifferentComponents()
    {
        // Arrange
        var point1 = new PointStub(1, 2);
        var point2 = new PointStub(2, 1); 

        // Act & Assert
        point1.GetHashCode().Should().NotBe(point2.GetHashCode());
    }
}