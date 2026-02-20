using System;
using System.Collections.Generic;
using Valora.Domain.Entities;

namespace Valora.Application.UseCases.Categories.UpdateSchema;

public record UpdateCategorySchemaCommand(
    Guid Id,
    List<CategoryFieldDto> Schema
);