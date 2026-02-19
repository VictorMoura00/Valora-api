using System;
using System.Collections.Generic;
using Valora.Domain.Common.Abstractions;
using Valora.Domain.Common.Exceptions;

namespace Valora.Domain.Entities;

public class Item : Entity, IAggregateRoot
{
    public Guid CategoryId { get; private set; }
    public Dictionary<string, object> Data { get; private set; }
    public decimal AverageRating { get; private set; }
    public int ReviewCount { get; private set; }

    // Construtor principal (Para uso da Aplicação)
    public Item(Guid categoryId, Dictionary<string, object> data)
    {
        CategoryId = categoryId;
        Data = data;
        AverageRating = 0;
        ReviewCount = 0;
    }

    // O MongoDB precisa que seja público para instanciar via Reflection sem configurações extras.
    [Obsolete("Construtor utilizado apenas pelo ORM/Database")]
    public Item() 
    {
        Data = new Dictionary<string, object>();
    }

    public void ValidateAgainstSchema(Category category)
    {
        foreach (var field in category.Schema)
        {
            if (field.IsRequired && !Data.ContainsKey(field.Name))
            {
                throw new DomainException($"O campo '{field.Name}' é obrigatório para a categoria '{category.Name}'.");
            }
        }
    }

    public void UpdateRating(int newRatingValue)
    {
        var totalScore = (AverageRating * ReviewCount) + newRatingValue;
        ReviewCount++;
        AverageRating = totalScore / ReviewCount;
        
        SetUpdated(); 
    }
}