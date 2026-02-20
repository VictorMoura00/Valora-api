using System;
using System.Collections.Generic;
using System.Text;
using Valora.Domain.Common.Abstractions;

namespace Valora.UnitTests.Common.Stub.ValueObjectStub;

public class Point3DStub : ValueObject
{
    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    public Point3DStub(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
        yield return Z;
    }
}
