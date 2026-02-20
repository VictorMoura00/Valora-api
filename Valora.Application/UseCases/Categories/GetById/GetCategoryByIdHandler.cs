using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Valora.Application.UseCases.Categories.Create;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Categories.GetById;

public static class GetCategoryByIdHandler
{
    public static async Task<Result<CategoryResponse>> Handle(
        GetCategoryByIdQuery query,
        ICategoryRepository categoryRepository,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(query.Id);

        if (category is null)
        {
            return Result.Failure<CategoryResponse>(Error.NotFound(
                "Category.NotFound",
                $"A categoria com o ID '{query.Id}' não foi encontrada."
            ));
        }

        var response = new CategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.Schema.Select(f => new CategoryFieldDto(f.Name, f.Type, f.IsRequired))
        );

        return response;
    }
}