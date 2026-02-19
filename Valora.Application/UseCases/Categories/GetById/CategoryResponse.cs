using System;
using Valora.Domain.Entities;

namespace Valora.Application.UseCases.Categories.GetById;

public record CategoryResponse(
    Guid Id,
    string Name,
    string Description,
    List<CategoryFieldResponse> Schema
);

public record CategoryFieldResponse(
    string Name, 
    string Type,
    bool IsRequired
);