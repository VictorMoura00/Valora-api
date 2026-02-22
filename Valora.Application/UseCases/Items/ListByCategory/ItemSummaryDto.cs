namespace Valora.Application.UseCases.Items.ListByCategory;

public record ItemSummaryDto(
    Guid Id,
    string Name,
    IReadOnlyDictionary<string, object> Attributes,
    DateTimeOffset CreatedAt
    );
