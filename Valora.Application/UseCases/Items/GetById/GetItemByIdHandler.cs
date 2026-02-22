using System.Threading;
using System.Threading.Tasks;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Items.GetById;

public static class GetItemByIdHandler
{
    public static async Task<Result<ItemDto>> Handle(
        GetItemByIdQuery query,
        IItemRepository itemRepository,
        CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByIdAsync(query.Id, cancellationToken);

        if (item is null)
            return Result.Failure<ItemDto>(Error.NotFound(
                "Item.NotFound",
                $"O item com o ID '{query.Id}' não foi encontrado."));

        var dto = new ItemDto(
            item.Id,
            item.CategoryId,
            item.Name,
            item.Attributes,
            item.CreatedAt,
            item.UpdatedAt);

        return Result.Success(dto);
    }
}
