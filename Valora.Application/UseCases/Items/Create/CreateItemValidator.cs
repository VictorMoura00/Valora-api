//using FluentValidation;

//namespace Valora.Application.UseCases.Items.Create;

//public class CreateItemValidator : AbstractValidator<CreateItemCommand>
//{
//    public CreateItemValidator()
//    {
//        RuleFor(x => x.CategoryId)
//            .NotEmpty().WithMessage("O ID da categoria é obrigatório.");

//        RuleFor(x => x.Fields)
//            .NotNull().WithMessage("Os campos do item são obrigatórios.")
//            .Must(x => x.Count > 0).WithMessage("O item deve conter pelo menos um campo preenchido.");
//    }
//}