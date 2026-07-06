using System.Security.Claims;
using System.Text.Json;
using Bff.API.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Bff.UnitTests;

public class CurrentUserTests
{
    private static CurrentUser Build(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, authenticationType: "Test");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        var accessor = new HttpContextAccessor { HttpContext = context };
        return new CurrentUser(accessor);
    }

    [Fact]
    public void SuperAdmin_can_access_any_business()
    {
        var sut = Build(new Claim(CurrentUser.SuperAdminClaim, "true"));

        sut.IsSuperAdmin.Should().BeTrue();
        sut.CanAccessBusiness("anything").Should().BeTrue();
    }

    [Fact]
    public void User_with_permission_can_access_only_that_business()
    {
        var permissions = JsonSerializer.Serialize(new[]
        {
            new { BusinessId = "biz-1", BusinessUnitId = (string?)null, Module = "Business", Function = (string?)null, Role = "Reader" },
        });
        var sut = Build(new Claim(CurrentUser.PermissionsClaim, permissions));

        sut.IsSuperAdmin.Should().BeFalse();
        sut.CanAccessBusiness("biz-1").Should().BeTrue();
        sut.CanAccessBusiness("biz-2").Should().BeFalse();
    }

    [Fact]
    public void No_permissions_claim_yields_empty_scope()
    {
        var sut = Build(new Claim("sub", "u-1"));

        sut.Permissions.Should().BeEmpty();
        sut.CanAccessBusiness("biz-1").Should().BeFalse();
    }

    [Fact]
    public void Malformed_permissions_claim_is_treated_as_empty()
    {
        var sut = Build(new Claim(CurrentUser.PermissionsClaim, "{ not valid json"));

        sut.Permissions.Should().BeEmpty();
        sut.CanAccessBusiness("biz-1").Should().BeFalse();
    }

    [Fact]
    public void Subject_is_read_from_nameidentifier_or_sub()
    {
        Build(new Claim(ClaimTypes.NameIdentifier, "abc")).Subject.Should().Be("abc");
        Build(new Claim("sub", "xyz")).Subject.Should().Be("xyz");
    }
}
