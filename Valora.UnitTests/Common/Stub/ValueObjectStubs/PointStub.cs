using System;
using System.Collections.Generic;
using System.Text;
using Valora.Domain.Common.Abstractions;

namespace Valora.UnitTests.Common.Stub.ValueObjectStubs;

public class PointStub(int x, int y) : ValueObject
{
    public int X { get; } = x;
    public int Y { get; } = y;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}