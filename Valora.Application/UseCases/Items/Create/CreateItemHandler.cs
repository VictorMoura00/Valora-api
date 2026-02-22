using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Valora.Application.UseCases.Items.Common;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Items.Create;

public static class CreateItemHandler
{
    public static async Task<Result<Guid>> Handle(
        CreateItemCommand command,
        ICategoryRepository categoryRepository,
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
        if (category is null)
            return Result.Failure<Guid>(Error.NotFound(
                "Item.CategoryNotFound",
                "A categoria informada não existe."));

        var exists = await itemRepository.ExistsByNameAsync(command.Name, command.CategoryId, cancellationToken);
        if (exists)
            return Result.Failure<Guid>(Error.Conflict(
                "Item.NameAlreadyExists",
                $"Já existe um item chamado '{command.Name}' nesta categoria."));

        var schemaValidationResult = ItemSchemaValidator.Validate(command.Attributes, category.Schema);
        if (schemaValidationResult.IsFailure)
            return Result.Failure<Guid>(schemaValidationResult.Error);

        var item = new Item(category.Id, command.Name);

        item.ReplaceAttributes(command.Attributes);

        await itemRepository.AddAsync(item, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(item.Id);
    }
}
