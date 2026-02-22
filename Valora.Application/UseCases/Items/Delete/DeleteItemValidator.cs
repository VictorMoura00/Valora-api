using FluentValidation;

namespace Valora.Application.UseCases.Items.Delete;

public class DeleteItemValidator : AbstractValidator<DeleteItemCommand>
{
    public DeleteItemValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do item é obrigatório.");
    }
}
