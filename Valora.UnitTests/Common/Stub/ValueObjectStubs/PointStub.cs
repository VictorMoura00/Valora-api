using System;
using System.Collections.Generic;
using System.Text;
using Valora.Domain.Common.Abstractions;

namespace Valora.UnitTests.Common.Stub.ValueObjectStub;

public class PointStub : ValueObject
{
    public int X { get; }
    public int Y { get; }

    public PointStub(int x, int y)
    {
        X = x;
        Y = y;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}