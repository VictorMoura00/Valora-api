using FluentValidation;

namespace Valora.Application.UseCases.Items.Update;

public class UpdateItemValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do item é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do item é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do item deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Attributes)
            .NotNull().WithMessage("A lista de atributos não pode ser nula.");
    }
}
