using System;
using System.Collections.Generic;
using Valora.Domain.Common.Abstractions;
using Valora.Domain.Common.Results;

namespace Valora.Domain.Entities;

public class Category : Entity, IAggregateRoot
{
    private readonly List<FieldDefinition> _schema = [];
    public string Name { get; private set; }
    public string Description { get; private set; }
    public IReadOnlyCollection<FieldDefinition> Schema => _schema.AsReadOnly();

    public Category(string name, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        Name = name;
        Description = description;
    }

    public Result Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Category.InvalidName", "O nome da categoria não pode ser vazio."));

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure(Error.Validation("Category.InvalidDescription", "A descrição não pode ser vazia."));

        Name = name;
        Description = description;

        SetUpdated();

        return Result.Success();
    }

    public Result AddField(string name, FieldType type, bool required = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Category.InvalidFieldName", "O nome do campo não pode ser vazio."));

        if (_schema.Exists(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return Result.Failure(Error.Conflict(
                "Category.DuplicateField",
                $"O campo '{name}' já existe no schema desta categoria."));

        _schema.Add(new FieldDefinition(name, type, required));

        SetUpdated();

        return Result.Success();
    }


    public Result ReplaceSchema(IEnumerable<FieldDefinition> newFields)
    {
        var tempSchema = new List<FieldDefinition>();

        foreach (var field in newFields)
        {
            if (string.IsNullOrWhiteSpace(field.Name))
                return Result.Failure(Error.Validation("Category.InvalidFieldName", "O nome do campo não pode ser vazio."));

            if (tempSchema.Exists(x => x.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase)))
                return Result.Failure(Error.Conflict(
                    "Category.DuplicateField",
                    $"O campo '{field.Name}' está duplicado na nova lista de schema."));

            tempSchema.Add(field);
        }

        _schema.Clear();
        _schema.AddRange(tempSchema);

        SetUpdated();

        return Result.Success();
    }
}


public record FieldDefinition(string Name, FieldType Type, bool IsRequired);

public enum FieldType { Text, Number, Date, Boolean, Select }
