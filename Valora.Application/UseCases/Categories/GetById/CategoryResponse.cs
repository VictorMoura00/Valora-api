using Valora.Application.UseCases.Categories.Create;

namespace Valora.Application.UseCases.Categories.GetById;

public record CategoryResponse(
    Guid Id,
    string Name,
    string Description,
    IEnumerable<CategoryFieldDto> Schema
);