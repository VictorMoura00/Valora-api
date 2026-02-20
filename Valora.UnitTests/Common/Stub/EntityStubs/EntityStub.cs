using Valora.Domain.Common.Abstractions;

public class EntityStub : Entity, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
}