using Valora.Domain.Common.Abstractions;

namespace Valora.Domain.Entities;

public class Review : Entity, IAggregateRoot
{
    public Guid ItemId { get; private set; } 
    public Guid UserId { get; private set; } 
    public decimal Rating { get; private set; } 
    public string? Comment { get; private set; }

    public Review(Guid itemId, Guid userId, decimal rating, string? comment)
    {
        if (rating < 0 || rating > 10) 
            throw new ArgumentException("Nota deve ser entre 0 e 10");

        ItemId = itemId;
        UserId = userId;
        Rating = rating;
        Comment = comment;
    }
}