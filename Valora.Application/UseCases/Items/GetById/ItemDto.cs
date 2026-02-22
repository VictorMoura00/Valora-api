namespace Valora.Application.UseCases.Items.GetById;

public record ItemDto(
    Guid Id,
    Guid CategoryId,
    string Name,
    IReadOnlyDictionary<string, object> Attributes,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
