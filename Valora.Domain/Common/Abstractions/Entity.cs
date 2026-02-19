namespace Valora.Domain.Common.Abstractions;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTimeOffset CreateAt { get; protected set; }
    public DateTimeOffset? UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
        CreateAt = DateTimeOffset.UtcNow;
        IsDeleted = false;
    }

    public void SetUpdated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        SetUpdated();
    }
    
}