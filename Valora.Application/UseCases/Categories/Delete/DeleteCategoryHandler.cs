using System.Threading;
using System.Threading.Tasks;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Categories.Delete;

public static class DeleteCategoryHandler
{
    public static async Task<Result> Handle(
        DeleteCategoryCommand command,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.Id, cancellationToken);

        if (category is null)
            return Result.Failure(Error.NotFound(
                "Category.NotFound",
                $"A categoria com o ID '{command.Id}' não foi encontrada."));

        await categoryRepository.DeleteAsync(category.Id, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
