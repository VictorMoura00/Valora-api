using System;
using System.Collections.Generic;
using System.Text;
using Valora.Domain.Common.Abstractions;

namespace Valora.UnitTests.Common.Stub.ValueObjectStubs;

public class Point3DStub(int x, int y, int z) : ValueObject
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public int Z { get; } = z;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
        yield return Z;
    }
}
