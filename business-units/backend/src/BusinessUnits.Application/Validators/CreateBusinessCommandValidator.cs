using BusinessUnits.Application.Commands.CreateBusiness;
using FluentValidation;

namespace BusinessUnits.Application.Validators;

public class CreateBusinessCommandValidator : AbstractValidator<CreateBusinessCommand>
{
    public CreateBusinessCommandValidator()
    {
        RuleFor(x => x.RazaoSocial).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NomeFantasia).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Cnpj).NotEmpty().Length(14, 18);
    }
}
