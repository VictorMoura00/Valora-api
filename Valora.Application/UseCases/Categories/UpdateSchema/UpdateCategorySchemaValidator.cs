using FluentValidation;

namespace Valora.Application.UseCases.Categories.UpdateSchema;

public class UpdateCategorySchemaValidator : AbstractValidator<UpdateCategorySchemaCommand>
{
    public UpdateCategorySchemaValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador da categoria é obrigatório.");

        RuleFor(x => x.Schema)
            .NotNull().WithMessage("A lista de schema não pode ser nula.");

        // Validação encadeada para cada item da lista (FluentValidation suporta isso nativamente)
        RuleForEach(x => x.Schema).ChildRules(field =>
        {
            field.RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do campo do schema é obrigatório.")
                .MaximumLength(50).WithMessage("O nome do campo não pode exceder 50 caracteres.");

            field.RuleFor(x => x.Type)
                .IsInEnum().WithMessage("O tipo do campo fornecido é inválido.");
        });
    }
}