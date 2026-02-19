using MediatR;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Categories.GetById;

public class GetCategoryByIdHandler(ICategoryRepository _categoryRepository) 
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryResponse>>
{
    public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);

        if (category is null)
            return Result.Failure<CategoryResponse>(Error.NotFound(
                "Category.NotFound",
                $"A categoria com o ID '{request.Id}' não foi encontrada."
            ));

        var response = new CategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.Schema.Select(f => new CategoryFieldResponse(
                f.Name,
                f.Type.ToString(),
                f.IsRequired
            )).ToList()
        );

        return response;
    }
}