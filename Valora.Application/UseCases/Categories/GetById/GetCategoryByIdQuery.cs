using MediatR;
using Valora.Domain.Common.Results;

namespace Valora.Application.UseCases.Categories.GetById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryResponse>>;