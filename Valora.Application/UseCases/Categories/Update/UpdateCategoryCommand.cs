using System;

namespace Valora.Application.UseCases.Categories.Update;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Description
);