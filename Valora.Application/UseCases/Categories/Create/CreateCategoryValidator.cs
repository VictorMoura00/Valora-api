using FluentValidation;

namespace Valora.Application.UseCases.Categories.Create;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(50).WithMessage("O nome não pode exceder 50 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(200);

        RuleForEach(x => x.Schema).ChildRules(field =>
        {
            field.RuleFor(f => f.Name)
                .NotEmpty().WithMessage("O nome do campo é obrigatório.");
            
            field.RuleFor(f => f.Type)
                .IsInEnum().WithMessage("Tipo de campo inválido.");
        });
    }
}