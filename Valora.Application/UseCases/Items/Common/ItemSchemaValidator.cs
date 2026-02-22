using System;
using System.Collections.Generic;
using System.Linq;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;

namespace Valora.Application.UseCases.Items.Common;

public static class ItemSchemaValidator
{
    /// <summary>
    /// Valida se os atributos enviados contêm todos os campos obrigatórios do Schema da Categoria.
    /// </summary>
    public static Result Validate(
        Dictionary<string, object> attributes,
        IReadOnlyCollection<FieldDefinition> schema)
    {
        var requiredFields = schema.Where(f => f.IsRequired).ToList();

        foreach (var requiredField in requiredFields)
        {
            var keyExists = attributes.Keys.Any(k => k.Equals(requiredField.Name, StringComparison.OrdinalIgnoreCase));

            if (!keyExists)
                return Result.Failure(Error.Validation(
                    "Item.MissingRequiredAttribute",
                    $"O campo '{requiredField.Name}' é obrigatório para esta categoria."));

            var value = attributes.FirstOrDefault(k => k.Key.Equals(requiredField.Name, StringComparison.OrdinalIgnoreCase)).Value;

            if (value is null || (value is string str && string.IsNullOrWhiteSpace(str)))
                return Result.Failure(Error.Validation(
                    "Item.EmptyRequiredAttribute",
                    $"O valor para o campo obrigatório '{requiredField.Name}' não pode ser vazio."));
        }

        return Result.Success();
    }
}
