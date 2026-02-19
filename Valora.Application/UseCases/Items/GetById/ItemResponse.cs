namespace Valora.Application.UseCases.Items.GetById;

public record ItemResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    Dictionary<string, object> Data,
    decimal AverageRating
);