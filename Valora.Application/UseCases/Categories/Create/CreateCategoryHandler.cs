using System;
using System.Threading;
using System.Threading.Tasks;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Categories.Create;

public static class CreateCategoryHandler
{
    public static async Task<Result<Guid>> Handle(
        CreateCategoryCommand command,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var existingCategory = await categoryRepository.GetByNameAsync(command.Name);
        
        if (existingCategory is not null)
        {
            return Result.Failure<Guid>(Error.Conflict(
                "Category.DuplicateName",
                $"Já existe uma categoria com o nome '{command.Name}'."
            ));
        }

        var category = new Category(command.Name, command.Description);

        if (command.Schema?.Count > 0)
        {
            foreach (var field in command.Schema)
            {
                category.AddField(field.Name, field.Type, field.IsRequired);
            }
        }

        await categoryRepository.AddAsync(category);
        await unitOfWork.CommitAsync(cancellationToken);

        return category.Id;
    }
}