using BusinessUnits.Application.Commands.CreateBusinessUnit;
using BusinessUnits.Domain.ValueObjects;
using FluentValidation;

namespace BusinessUnits.Application.Validators;

public class CreateBusinessUnitCommandValidator : AbstractValidator<CreateBusinessUnitCommand>
{
    public CreateBusinessUnitCommandValidator()
    {
        RuleFor(x => x.RazaoSocial).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NomeFantasia).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .Must(cnpj => Cnpj.IsValid(new string(cnpj.Where(char.IsDigit).ToArray())))
            .WithMessage("CNPJ inválido.");

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address!.Street).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Address!.Number).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Address!.District).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Address!.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Address!.State).NotEmpty().Length(2);
            RuleFor(x => x.Address!.ZipCode).NotEmpty().Matches(@"^\d{5}-?\d{3}$").WithMessage("CEP inválido.");
        });

        When(x => x.Contacts is not null && x.Contacts.Any(), () =>
        {
            RuleForEach(x => x.Contacts).ChildRules(contact =>
            {
                contact.RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
                contact.RuleFor(c => c.Email).NotEmpty().EmailAddress();
                contact.RuleFor(c => c.Phone).NotEmpty().MaximumLength(20);
            });
        });
    }
}
