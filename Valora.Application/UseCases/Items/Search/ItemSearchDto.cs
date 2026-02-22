namespace Valora.Application.UseCases.Items.Search;

public record ItemSearchDto(
    Guid Id,
    Guid CategoryId,
    string Name,
    IReadOnlyDictionary<string, object> Attributes);
