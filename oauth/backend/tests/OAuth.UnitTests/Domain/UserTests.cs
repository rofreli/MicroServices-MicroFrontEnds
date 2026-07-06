using OAuth.Domain.Constants;
using OAuth.Domain.Entities;
using OAuth.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace OAuth.UnitTests.Domain;

public class UserTests
{
    private static User NewUser(bool superAdmin = false)
        => User.Create("ana@acme.com", "Ana", "Lima", "hash", superAdmin);

    private static Permission Perm(
        string businessId = "biz-1", string? buId = null,
        string module = "Business", string? function = null, string role = "Reader")
        => Permission.Create(businessId, buId, module, function, role);

    [Fact]
    public void Create_sets_defaults()
    {
        var user = NewUser();

        user.Id.Should().NotBeNullOrEmpty();
        user.Email.Value.Should().Be("ana@acme.com");
        user.FullName.Should().Be("Ana Lima");
        user.IsActive.Should().BeTrue();
        user.IsSuperAdmin.Should().BeFalse();
        user.Permissions.Should().BeEmpty();
    }

    [Fact]
    public void Create_can_flag_super_admin()
    {
        NewUser(superAdmin: true).IsSuperAdmin.Should().BeTrue();
    }

    [Fact]
    public void AddPermission_adds_new_scope()
    {
        var user = NewUser();

        user.AddPermission(Perm(module: OAuthModules.Business, role: OAuthRoles.Reader));
        user.AddPermission(Perm(module: OAuthModules.Users, role: OAuthRoles.Writer));

        user.Permissions.Should().HaveCount(2);
    }

    [Fact]
    public void AddPermission_replaces_role_on_same_scope()
    {
        var user = NewUser();

        user.AddPermission(Perm(module: OAuthModules.Business, role: OAuthRoles.Reader));
        user.AddPermission(Perm(module: OAuthModules.Business, role: OAuthRoles.Manager));

        user.Permissions.Should().ContainSingle()
            .Which.Role.Should().Be(OAuthRoles.Manager);
    }

    [Fact]
    public void RemovePermission_removes_matching_scope()
    {
        var user = NewUser();
        user.AddPermission(Perm(module: OAuthModules.Business));

        user.RemovePermission("biz-1", null, OAuthModules.Business, null);

        user.Permissions.Should().BeEmpty();
    }

    [Fact]
    public void RemovePermission_throws_when_scope_absent()
    {
        var user = NewUser();

        var act = () => user.RemovePermission("biz-1", null, OAuthModules.Business, null);

        act.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void Deactivate_twice_throws()
    {
        var user = NewUser();
        user.Deactivate();

        var act = () => user.Deactivate();

        act.Should().Throw<DomainException>();
    }
}
