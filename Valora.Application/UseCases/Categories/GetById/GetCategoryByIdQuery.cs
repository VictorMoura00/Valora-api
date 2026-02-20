using System;
using System.Collections.Generic;
using Valora.Application.UseCases.Categories.Create; // Reaproveitando o CategoryFieldDto (DRY)

namespace Valora.Application.UseCases.Categories.GetById;

public record GetCategoryByIdQuery(Guid Id);