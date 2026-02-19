using System;
using System.Collections.Generic;
using Valora.Domain.Common.Abstractions;
using Valora.Domain.Common.Exceptions; 

namespace Valora.Domain.Entities;

public class Item(Guid categoryId, Dictionary<string, object> data) : Entity, IAggregateRoot
{
    public Guid CategoryId { get; private set; } = categoryId;

    public Dictionary<string, object> Data { get; private set; } = data;

    public decimal AverageRating { get; private set; } = 0;
    public int ReviewCount { get; private set; } = 0;

    /// <summary>
    /// Valida se os dados deste item respeitam o "Schema" da categoria.
    /// </summary>
    public void ValidateAgainstSchema(Category category)
    {
        foreach (var field in category.Schema)
        {
            // Verifica se campos obrigatórios estão presentes
            if (field.IsRequired && !Data.ContainsKey(field.Name))
            {
                throw new DomainException($"O campo '{field.Name}' é obrigatório para a categoria '{category.Name}'.");
            }

            // Aqui você poderá expandir para validar Tipos (se é número, data, etc)
        }
    }

    /// <summary>
    /// Recalcula a média de avaliações.
    /// </summary>
    public void UpdateRating(int newRatingValue)
    {
        var totalScore = (AverageRating * ReviewCount) + newRatingValue;
        ReviewCount++;
        AverageRating = totalScore / ReviewCount;
        
        SetUpdated(); 
    }
}