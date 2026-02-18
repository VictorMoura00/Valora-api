namespace Valora.Application.UseCases.Items.GetById;

public record ItemResponse(
    Guid Id,
    string Name,
    string Description,
    string Category,
    decimal AverageRating
);