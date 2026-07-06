using BusinessUnits.Application.Commands.CreateBusiness;
using BusinessUnits.Application.Validators;
using FluentAssertions;
using Xunit;

namespace BusinessUnits.UnitTests.Application;

public class CreateBusinessCommandValidatorTests
{
    private readonly CreateBusinessCommandValidator _validator = new();

    [Fact]
    public void Valid_command_passes()
    {
        var result = _validator.Validate(
            new CreateBusinessCommand("Acme SA", "Acme", "11222333000181"));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Acme", "11222333000181")]      // empty razao social
    [InlineData("Acme SA", "", "11222333000181")]   // empty nome fantasia
    [InlineData("Acme SA", "Acme", "123")]          // cnpj too short
    public void Invalid_command_fails(string razao, string fantasia, string cnpj)
    {
        var result = _validator.Validate(new CreateBusinessCommand(razao, fantasia, cnpj));

        result.IsValid.Should().BeFalse();
    }
}
