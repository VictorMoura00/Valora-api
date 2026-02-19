using Valora.Domain.Common.Abstractions;

namespace Valora.Domain.Entities;

public class Category(string name, string description) : Entity, IAggregateRoot
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    
    public List<FieldDefinition> Schema { get; private set; } = [];

    public void AddField(string name, FieldType type, bool required = false)
    {
        if (Schema.Exists(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            return; 
        }

        Schema.Add(new FieldDefinition(name, type, required));
    }
}

public record FieldDefinition(string Name, FieldType Type, bool IsRequired);

public enum FieldType { Text, Number, Date, Boolean, Select }