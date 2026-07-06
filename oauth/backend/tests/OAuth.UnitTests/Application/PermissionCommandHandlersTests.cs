using OAuth.Application.Commands.AddRole;
using OAuth.Application.Commands.RemoveRole;
using OAuth.Domain.Constants;
using OAuth.Domain.Entities;
using OAuth.Domain.Exceptions;
using OAuth.Domain.Interfaces;
using OAuth.UnitTests.TestSupport;
using FluentAssertions;
using Moq;
using Xunit;

namespace OAuth.UnitTests.Application;

public class PermissionCommandHandlersTests
{
    private readonly Mock<IUserRepository> _repository = new();

    private static User NewUser()
        => User.Create("ana@acme.com", "Ana", "Lima", "hash");

    // ── Add ──────────────────────────────────────────────────────────────────────

    private AddPermissionToUserCommandHandler AddHandler()
        => new(_repository.Object, MapperFactory.Create());

    [Fact]
    public async Task Add_throws_when_user_missing()
    {
        _repository.Setup(x => x.GetByIdAsync("u-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new AddPermissionToUserCommand(
            "u-1", "biz-1", null, OAuthModules.Business, null, OAuthRoles.Reader);
        var act = () => AddHandler().Handle(command, default);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Add_persists_permission_on_user()
    {
        var user = NewUser();
        _repository.Setup(x => x.GetByIdAsync("u-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new AddPermissionToUserCommand(
            "u-1", "biz-1", null, OAuthModules.Business, null, OAuthRoles.Manager);
        var result = await AddHandler().Handle(command, default);

        user.Permissions.Should().ContainSingle()
            .Which.Role.Should().Be(OAuthRoles.Manager);
        result.Permissions.Should().ContainSingle();
        _repository.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── Remove ───────────────────────────────────────────────────────────────────

    private RemovePermissionFromUserCommandHandler RemoveHandler()
        => new(_repository.Object, MapperFactory.Create());

    [Fact]
    public async Task Remove_throws_when_user_missing()
    {
        _repository.Setup(x => x.GetByIdAsync("u-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new RemovePermissionFromUserCommand(
            "u-1", "biz-1", null, OAuthModules.Business, null);
        var act = () => RemoveHandler().Handle(command, default);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Remove_deletes_matching_permission()
    {
        var user = NewUser();
        user.AddPermission(Permission.Create("biz-1", null, OAuthModules.Business, null, OAuthRoles.Reader));
        _repository.Setup(x => x.GetByIdAsync("u-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new RemovePermissionFromUserCommand(
            "u-1", "biz-1", null, OAuthModules.Business, null);
        await RemoveHandler().Handle(command, default);

        user.Permissions.Should().BeEmpty();
        _repository.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}
