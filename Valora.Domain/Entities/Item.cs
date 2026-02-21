using System.Collections.ObjectModel;
using Valora.Domain.Common.Abstractions;
using Valora.Domain.Common.Results;

namespace Valora.Domain.Entities;

public class Item : Entity, IAggregateRoot
{
    public Guid CategoryId { get; private set; }
    public string Name { get; private set; }
    private readonly Dictionary<string, object> _attributes = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyDictionary<string, object> Attributes => new ReadOnlyDictionary<string, object>(_attributes);

    public Item(Guid categoryId, string name)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("A categoria é obrigatória.", nameof(categoryId));

        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        CategoryId = categoryId;
        Name = name;
    }

    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Item.InvalidName", "O nome do item não pode ser vazio."));

        Name = name;
        SetUpdated();

        return Result.Success();
    }

    /// <summary>
    /// Adiciona ou atualiza um atributo dinâmico do item.
    /// A validação de se a 'key' existe no Schema da Categoria será feita pelo Handler/DomainService.
    /// </summary>
    public void SetAttribute(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key)) return;

        if (value is null || (value is string str && string.IsNullOrWhiteSpace(str)))
            _attributes.Remove(key);
        else
            _attributes[key] = value;

        SetUpdated();
    }

    /// <summary>
    /// Substitui todos os atributos de uma vez (útil para formulários de edição).
    /// </summary>
    public void ReplaceAttributes(IDictionary<string, object> newAttributes)
    {
        _attributes.Clear();

        foreach (var attr in newAttributes)
            SetAttribute(attr.Key, attr.Value);
    }
}
