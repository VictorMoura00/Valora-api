using System.Threading;
using System.Threading.Tasks;
using Valora.Domain.Common.Pagination;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Items.ListByCategory;

public static class ListItemsByCategoryHandler
{
    public static async Task<Result<PaginatedList<ItemSummaryDto>>> Handle(
        ListItemsByCategoryQuery query,
        ICategoryRepository categoryRepository,
        IItemRepository itemRepository,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(query.CategoryId, cancellationToken);

        if (category is null)
            return Result.Failure<PaginatedList<ItemSummaryDto>>(Error.NotFound(
                "Category.NotFound",
                "A categoria especificada não foi encontrada."));

        var paginatedItems = await itemRepository.GetPaginatedByCategoryAsync(
            query.CategoryId,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        // 3. Usa o método Map() que refatoramos para converter de Entidade para DTO de forma limpa
        var paginatedDtos = paginatedItems.Map(item => new ItemSummaryDto(
            item.Id,
            item.Name,
            item.Attributes,
            item.CreatedAt));

        return Result.Success(paginatedDtos);
    }
}
