using System.Threading;
using System.Threading.Tasks;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Items.Delete;

public static class DeleteItemHandler
{
    public static async Task<Result> Handle(
        DeleteItemCommand command,
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByIdAsync(command.Id, cancellationToken);

        if (item is null)
            return Result.Failure(Error.NotFound(
                "Item.NotFound",
                $"O item com o ID '{command.Id}' não foi encontrado."));

        await itemRepository.DeleteAsync(item.Id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
