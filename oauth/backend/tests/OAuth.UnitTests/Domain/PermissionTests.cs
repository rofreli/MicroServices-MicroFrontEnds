using OAuth.Domain.Constants;
using OAuth.Domain.Entities;
using OAuth.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace OAuth.UnitTests.Domain;

public class PermissionTests
{
    [Fact]
    public void Create_valid_permission()
    {
        var permission = Permission.Create(
            "biz-1", null, OAuthModules.Business, null, OAuthRoles.Writer);

        permission.BusinessId.Should().Be("biz-1");
        permission.Module.Should().Be(OAuthModules.Business);
        permission.Role.Should().Be(OAuthRoles.Writer);
    }

    [Fact]
    public void Create_requires_business_id()
    {
        var act = () => Permission.Create("", null, OAuthModules.Business, null, OAuthRoles.Reader);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_rejects_unknown_module()
    {
        var act = () => Permission.Create("biz-1", null, "Ghost", null, OAuthRoles.Reader);

        act.Should().Throw<DomainException>().WithMessage("*module*");
    }

    [Fact]
    public void Create_rejects_unknown_role()
    {
        var act = () => Permission.Create("biz-1", null, OAuthModules.Users, null, "Overlord");

        act.Should().Throw<DomainException>().WithMessage("*role*");
    }

    [Fact]
    public void Create_rejects_unknown_function()
    {
        var act = () => Permission.Create("biz-1", null, OAuthModules.Users, "Teleport", OAuthRoles.Manager);

        act.Should().Throw<DomainException>().WithMessage("*function*");
    }

    [Fact]
    public void Blank_business_unit_id_is_normalized_to_null_for_matching()
    {
        var permission = Permission.Create("biz-1", "  ", OAuthModules.Business, null, OAuthRoles.Reader);

        permission.BusinessUnitId.Should().BeNull();
        permission.Matches("biz-1", null, OAuthModules.Business, null).Should().BeTrue();
    }
}
