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

namespace Valora.Application.UseCases.Items.Update;

public static class UpdateItemHandler
{
    public static async Task<Result> Handle(
        UpdateItemCommand command,
        ICategoryRepository categoryRepository,
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        // 1. Busca o item existente
        var item = await itemRepository.GetByIdAsync(command.Id, cancellationToken);
        if (item is null)
            return Result.Failure(Error.NotFound(
                "Item.NotFound",
                $"O item com o ID '{command.Id}' não foi encontrado."));

        // 2. Validação de unicidade do nome (apenas se o nome estiver sendo alterado)
        if (!item.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await itemRepository.ExistsByNameAsync(command.Name, item.CategoryId, cancellationToken);
            if (exists)
                return Result.Failure(Error.Conflict(
                    "Item.NameAlreadyExists",
                    $"Já existe um item chamado '{command.Name}' nesta categoria."));
        }

        // 3. Busca a Categoria para recuperar o Schema
        var category = await categoryRepository.GetByIdAsync(item.CategoryId, cancellationToken);
        if (category is null)
            return Result.Failure(Error.NotFound(
                "Item.CategoryNotFound",
                "A categoria atrelada a este item não existe mais."));

        var schemaValidationResult = ItemSchemaValidator.Validate(command.Attributes, category.Schema);
        if (schemaValidationResult.IsFailure)
            return Result.Failure(schemaValidationResult.Error);

        // 5. Aplica as atualizações na Entidade
        var updateNameResult = item.UpdateName(command.Name);
        if (updateNameResult.IsFailure)
            return Result.Failure(updateNameResult.Error);

        item.ReplaceAttributes(command.Attributes);

        // 6. Persistência e Transação
        await itemRepository.UpdateAsync(item, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
