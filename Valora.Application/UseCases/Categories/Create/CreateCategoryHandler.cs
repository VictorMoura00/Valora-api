using System;
using System.Threading;
using System.Threading.Tasks;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Categories.Create;

public class CreateCategoryHandler(
    ICategoryRepository _categoryRepository,
    IUnitOfWork _unitOfWork
)
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var existingCategory = await _categoryRepository.GetByNameAsync(command.Name);
        if (existingCategory is not null)
        {
            return Result.Failure<Guid>(Error.Conflict(
                "Category.DuplicateName",
                $"Já existe uma categoria com o nome '{command.Name}'."
            ));
        }

        var category = new Category(command.Name, command.Description);

        if (command.Schema is not null)
        {
            foreach (var field in command.Schema)
            {
                category.AddField(field.Name, field.Type, field.IsRequired);
            }
        }

        await _categoryRepository.AddAsync(category);
        await _unitOfWork.CommitAsync(cancellationToken);

        return category.Id;
    }
}