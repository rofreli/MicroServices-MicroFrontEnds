using BusinessUnits.Domain.Entities;
using BusinessUnits.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace BusinessUnits.UnitTests.Domain;

public class BusinessUnitTests
{
    private const string ValidCnpj = "11.333.444/0001-65";

    [Fact]
    public void Create_requires_a_parent_business()
    {
        var act = () => BusinessUnit.Create("", "Filial SP", "SP", ValidCnpj);

        act.Should().Throw<DomainException>()
            .WithMessage("*requires a Business*");
    }

    [Fact]
    public void Create_sets_business_id_and_fields()
    {
        var unit = BusinessUnit.Create("biz-1", "Filial SP", "SP", ValidCnpj);

        unit.Id.Should().NotBeNullOrEmpty();
        unit.BusinessId.Should().Be("biz-1");
        unit.RazaoSocial.Should().Be("Filial SP");
        unit.Cnpj.Value.Should().Be(ValidCnpj);
    }

    [Fact]
    public void AddContact_then_RemoveContact_maintains_collection()
    {
        var unit = BusinessUnit.Create("biz-1", "Filial SP", "SP", ValidCnpj);

        var contact = unit.AddContact("Ana", "ana@acme.com", "+55 11 99999-0000", ContactType.Primary);
        unit.Contacts.Should().ContainSingle();

        unit.RemoveContact(contact.Id);
        unit.Contacts.Should().BeEmpty();
    }

    [Fact]
    public void RemoveContact_with_unknown_id_throws()
    {
        var unit = BusinessUnit.Create("biz-1", "Filial SP", "SP", ValidCnpj);

        var act = () => unit.RemoveContact("nope");

        act.Should().Throw<NotFoundException>();
    }
}
