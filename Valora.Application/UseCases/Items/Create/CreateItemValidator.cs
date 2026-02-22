using FluentValidation;

namespace Valora.Application.UseCases.Items.Create;

public class CreateItemValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("O identificador da categoria é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do item é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do item deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Attributes)
            .NotNull().WithMessage("A lista de atributos não pode ser nula.");
    }
}
