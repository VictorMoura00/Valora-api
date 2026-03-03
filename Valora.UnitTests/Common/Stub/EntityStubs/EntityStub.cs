using Valora.Domain.Common.Abstractions;

namespace Valora.UnitTests.Common.Stub.EntityStubs;

public class EntityStub : Entity, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
}