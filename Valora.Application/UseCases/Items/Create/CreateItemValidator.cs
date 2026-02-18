using FluentValidation;

namespace Valora.Application.UseCases.Items.Create;

public class CreateItemValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do item é obrigatório.")
            .MaximumLength(100);

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("A categoria é obrigatória.");
    }
}