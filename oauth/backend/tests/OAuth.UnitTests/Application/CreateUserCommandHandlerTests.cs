using OAuth.Application.Commands.CreateUser;
using OAuth.Domain.Entities;
using OAuth.Domain.Exceptions;
using OAuth.Domain.Interfaces;
using OAuth.UnitTests.TestSupport;
using FluentAssertions;
using Moq;
using Xunit;

namespace OAuth.UnitTests.Application;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repository = new();
    private readonly Mock<IPasswordHasher> _hasher = new();

    public CreateUserCommandHandlerTests()
    {
        _hasher.Setup(x => x.Hash(It.IsAny<string>())).Returns("hashed");
    }

    private CreateUserCommandHandler CreateHandler()
        => new(_repository.Object, _hasher.Object, MapperFactory.Create());

    private static CreateUserCommand Command()
        => new("ana@acme.com", "Ana", "Lima", "Str0ngPass");

    [Fact]
    public async Task Handle_creates_user_hashes_password_and_maps_dto()
    {
        _repository.Setup(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await CreateHandler().Handle(Command(), default);

        result.Email.Should().Be("ana@acme.com");
        result.FullName.Should().Be("Ana Lima");
        result.IsSuperAdmin.Should().BeFalse();
        _hasher.Verify(x => x.Hash("Str0ngPass"), Times.Once);
        _repository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_rejects_duplicate_email()
    {
        _repository.Setup(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => CreateHandler().Handle(Command(), default);

        await act.Should().ThrowAsync<ConflictException>();
        _repository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
