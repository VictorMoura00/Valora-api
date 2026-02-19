using System;
using MediatR;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;

namespace Valora.Application.UseCases.Categories.Create;

public record CategoryFieldDto(string Name, FieldType Type, bool IsRequired);

public record CreateCategoryCommand(
    string Name, 
    string Description, 
    List<CategoryFieldDto> Schema
) : IRequest<Result<Guid>>;