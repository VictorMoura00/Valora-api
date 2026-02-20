using FluentValidation;

namespace Valora.Application.UseCases.Categories.Update;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador da categoria é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(50).WithMessage("O nome não pode exceder 50 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(200).WithMessage("A descrição não pode exceder 200 caracteres.");
    }
}