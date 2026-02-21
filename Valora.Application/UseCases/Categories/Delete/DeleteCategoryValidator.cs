using FluentValidation;

namespace Valora.Application.UseCases.Categories.Delete;

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador da categoria é obrigatório.");
    }
}
