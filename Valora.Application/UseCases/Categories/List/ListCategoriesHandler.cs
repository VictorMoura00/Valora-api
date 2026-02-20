using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Valora.Application.UseCases.Categories.Create;
using Valora.Application.UseCases.Categories.GetById;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Categories.List;

public static class ListCategoriesHandler
{
    public static async Task<Result<PaginatedList<CategoryResponse>>> Handle(
        ListCategoriesQuery query,
        ICategoryRepository categoryRepository,
        CancellationToken cancellationToken)
    {
        var paginatedCategories = await categoryRepository.GetPaginatedAsync(
            query.Page, 
            query.PageSize, 
            cancellationToken);

        var mappedItems = paginatedCategories.Items.Select(category => new CategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.Schema.Select(f => new CategoryFieldDto(f.Name, f.Type, f.IsRequired))
        )).ToList();

        var response = new PaginatedList<CategoryResponse>(
            mappedItems, 
            paginatedCategories.TotalCount, 
            paginatedCategories.PageNumber, 
            paginatedCategories.PageSize);

        return response;
    }
}