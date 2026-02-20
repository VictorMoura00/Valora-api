using System;
using System.Collections.Generic;
using Valora.Domain.Common.Abstractions;

namespace Valora.Domain.Entities;

public class Category : Entity, IAggregateRoot
{
    private readonly List<FieldDefinition> _schema = [];
    public string Name { get; private set; }
    public string Description { get; private set; }
    public IReadOnlyCollection<FieldDefinition> Schema => _schema.AsReadOnly();

    public Category(string name, string description)
    {
        // DRY: Reutilizando as validações e atribuições
        Update(name, description);
    }

    public void Update(string name, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        Name = name;
        Description = description;

        SetUpdated();
    }

    public void AddField(string name, FieldType type, bool required = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_schema.Exists(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"O campo '{name}' já existe no schema desta categoria.");

        _schema.Add(new FieldDefinition(name, type, required));

        SetUpdated();
    }
}

public record FieldDefinition(string Name, FieldType Type, bool IsRequired);

public enum FieldType { Text, Number, Date, Boolean, Select }