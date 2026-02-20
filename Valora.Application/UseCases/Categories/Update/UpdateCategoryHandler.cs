using System.Threading;
using System.Threading.Tasks;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Categories.Update;

public static class UpdateCategoryHandler
{
    public static async Task<Result> Handle(
        UpdateCategoryCommand command,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.Id);

        if (category is null)
            return Result.Failure(Error.NotFound("Category.NotFound",
                                                $"A categoria com o ID '{command.Id}' não foi encontrada."));

        var existingCategoryWithSameName = await categoryRepository.GetByNameAsync(command.Name);

        if (existingCategoryWithSameName is not null && existingCategoryWithSameName.Id != command.Id)
            return Result.Failure(Error.Conflict("Category.DuplicateName",
                                                $"Já existe outra categoria utilizando o nome '{command.Name}'."));

        category.Update(command.Name, command.Description);

        await categoryRepository.UpdateAsync(category);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}