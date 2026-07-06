using BusinessUnits.Domain.Entities;
using BusinessUnits.Domain.Events;
using BusinessUnits.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace BusinessUnits.UnitTests.Domain;

public class BusinessTests
{
    [Fact]
    public void Create_initializes_active_business_with_id_and_event()
    {
        var business = Business.Create("Acme SA", "Acme", "11.222.333/0001-81");

        business.Id.Should().NotBeNullOrEmpty();
        business.RazaoSocial.Should().Be("Acme SA");
        business.NomeFantasia.Should().Be("Acme");
        business.Cnpj.Value.Should().Be("11.222.333/0001-81");
        business.IsActive.Should().BeTrue();
        business.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<BusinessCreatedEvent>();
    }

    [Fact]
    public void Update_changes_names_and_stamps_updatedAt()
    {
        var business = Business.Create("Acme SA", "Acme", "11.222.333/0001-81");

        business.Update("Acme Holdings SA", "Acme Holdings");

        business.RazaoSocial.Should().Be("Acme Holdings SA");
        business.NomeFantasia.Should().Be("Acme Holdings");
        business.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_then_reactivate_toggles_state()
    {
        var business = Business.Create("Acme SA", "Acme", "11.222.333/0001-81");

        business.Deactivate();
        business.IsActive.Should().BeFalse();

        business.Activate();
        business.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_twice_throws()
    {
        var business = Business.Create("Acme SA", "Acme", "11.222.333/0001-81");
        business.Deactivate();

        var act = () => business.Deactivate();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Activate_when_already_active_throws()
    {
        var business = Business.Create("Acme SA", "Acme", "11.222.333/0001-81");

        var act = () => business.Activate();

        act.Should().Throw<DomainException>();
    }
}
