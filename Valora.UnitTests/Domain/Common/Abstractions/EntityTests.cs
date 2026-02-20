using System;
using Xunit;
using Valora.Domain.Common.Abstractions;

namespace Valora.UnitTests.Domain.Common.Abstractions;

public class StubEntity : Entity { }

public class EntityTests
{
    [Fact]
    public void Equals_DeveRetornarTrue_QuandoIdsForemIguais()
    {
        // Arrange
        var entity1 = new StubEntity();
        var entity2 = entity1; // Mesma referência e mesmo ID

        // Act & Assert
        Assert.True(entity1.Equals(entity2));
        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }

    [Fact]
    public void Equals_DeveRetornarFalse_QuandoIdsForemDiferentes()
    {
        // Arrange
        var entity1 = new StubEntity();
        var entity2 = new StubEntity();

        // Act & Assert
        Assert.False(entity1.Equals(entity2));
        Assert.NotEqual(entity1.GetHashCode(), entity2.GetHashCode());
    }

    [Fact]
    public void Delete_DeveMarcarComoDeletado_E_PreencherUpdatedAt()
    {
        // Arrange
        var entity = new StubEntity();
        Assert.Null(entity.UpdatedAt);
        Assert.False(entity.IsDeleted);

        // Act
        entity.Delete();

        // Assert
        Assert.True(entity.IsDeleted);
        Assert.NotNull(entity.UpdatedAt);
    }
}