using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Categories.UpdateSchema;

public static class UpdateCategorySchemaHandler
{
    public static async Task<Result> Handle(
        UpdateCategorySchemaCommand command,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.Id);

        if (category is null)
            return Result.Failure(Error.NotFound("Category.NotFound",
                $"A categoria com o ID '{command.Id}' não foi encontrada."));

        var domainFields = command.Schema
            .Select(dto => new FieldDefinition(dto.Name, dto.Type, dto.IsRequired))
            .ToList();

        var updateResult = category.ReplaceSchema(domainFields);

        if (updateResult.IsFailure)
            return updateResult;

        await categoryRepository.UpdateAsync(category);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}