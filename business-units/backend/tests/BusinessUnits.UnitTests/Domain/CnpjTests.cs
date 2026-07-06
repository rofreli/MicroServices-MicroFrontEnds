using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BusinessUnits.UnitTests.Domain;

public class CnpjTests
{
    [Theory]
    [InlineData("11222333000181")]
    [InlineData("11.222.333/0001-81")]
    [InlineData("11 222 333 0001 81")]
    public void Accepts_valid_cnpj_and_normalizes_formatting(string input)
    {
        var cnpj = new Cnpj(input);

        cnpj.Value.Should().Be("11.222.333/0001-81");
    }

    [Theory]
    [InlineData("12345678000100")]  // wrong check digits
    [InlineData("11111111111111")]  // all same digits
    [InlineData("123")]             // too short
    [InlineData("")]
    public void Rejects_invalid_cnpj(string input)
    {
        var act = () => new Cnpj(input);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Equality_is_by_value()
    {
        new Cnpj("11.222.333/0001-81").Should().Be(new Cnpj("11222333000181"));
    }
}
