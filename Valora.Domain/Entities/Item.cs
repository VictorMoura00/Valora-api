using Valora.Domain.Common.Abstractions;

namespace Valora.Domain.Entities;

public class Item : Entity, IAggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Category { get; private set; }
    
    //Dados cacheados
    public decimal AverageRating { get; private set; }
    public int ReviewCount { get; private set; }
    
    public Item(string name, string description, string category)
    {
        Name = name;
        Description = description;
        Category = category;
        AverageRating = 0;
        ReviewCount = 0;
    }

    public void UpdateRating(decimal newRatingValue)
    {
        var totalScore = (AverageRating * ReviewCount) + newRatingValue;
        ReviewCount++;
        AverageRating = totalScore / ReviewCount;
        
        SetUdapted();
    }
}