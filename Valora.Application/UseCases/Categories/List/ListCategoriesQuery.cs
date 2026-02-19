using MediatR;
using Valora.Application.UseCases.Categories.GetById;
using Valora.Domain.Common.Results;

namespace Valora.Application.UseCases.Categories.List;

public record ListCategoriesQuery(int Page = 1, int PageSize = 10) 
    : IRequest<Result<PaginatedList<CategoryResponse>>>;