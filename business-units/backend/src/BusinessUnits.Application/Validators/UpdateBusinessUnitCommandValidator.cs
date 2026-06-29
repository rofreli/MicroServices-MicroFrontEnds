using BusinessUnits.Application.Commands.UpdateBusinessUnit;
using FluentValidation;

namespace BusinessUnits.Application.Validators;

public class UpdateBusinessUnitCommandValidator : AbstractValidator<UpdateBusinessUnitCommand>
{
    public UpdateBusinessUnitCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.RazaoSocial).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NomeFantasia).NotEmpty().MaximumLength(200);

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address!.Street).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Address!.Number).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Address!.District).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Address!.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Address!.State).NotEmpty().Length(2);
            RuleFor(x => x.Address!.ZipCode).NotEmpty().Matches(@"^\d{5}-?\d{3}$").WithMessage("CEP inválido.");
        });
    }
}
