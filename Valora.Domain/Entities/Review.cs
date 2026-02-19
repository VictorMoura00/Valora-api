using Valora.Domain.Common.Abstractions;

namespace Valora.Domain.Entities;

public class Review : Entity, IAggregateRoot
{
    public Guid ItemId { get; private set; }
    public Guid UserId { get; private set; }
    public int Rating { get; private set; } // 1 a 5
    public string Comment { get; private set; }
}