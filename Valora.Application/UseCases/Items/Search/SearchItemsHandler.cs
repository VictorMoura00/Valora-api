using System.Threading;
using System.Threading.Tasks;
using Valora.Domain.Common.Pagination;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Items.Search;

public static class SearchItemsHandler
{
    public static async Task<Result<PaginatedList<ItemSearchDto>>> Handle(
        SearchItemsQuery query,
        IItemRepository itemRepository,
        CancellationToken cancellationToken)
    {
        var paginatedItems = await itemRepository.SearchByNameAsync(
            query.SearchTerm,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        var paginatedDtos = paginatedItems.Map(item => new ItemSearchDto(
            item.Id,
            item.CategoryId,
            item.Name,
            item.Attributes));

        return Result.Success(paginatedDtos);
    }
}
