using BusinessUnits.Application.Commands.CreateBusiness;
using BusinessUnits.Domain.Entities;
using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.Interfaces;
using BusinessUnits.UnitTests.TestSupport;
using FluentAssertions;
using Moq;
using Xunit;

namespace BusinessUnits.UnitTests.Application;

public class CreateBusinessCommandHandlerTests
{
    private readonly Mock<IBusinessRepository> _repository = new();

    private CreateBusinessCommandHandler CreateHandler()
        => new(_repository.Object, MapperFactory.Create());

    [Fact]
    public async Task Handle_persists_business_and_returns_dto()
    {
        _repository.Setup(x => x.ExistsByCnpjAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateBusinessCommand("Acme SA", "Acme", "11.222.333/0001-81");
        var result = await CreateHandler().Handle(command, default);

        result.RazaoSocial.Should().Be("Acme SA");
        result.Cnpj.Should().Be("11.222.333/0001-81");
        result.IsActive.Should().BeTrue();
        _repository.Verify(x => x.AddAsync(It.IsAny<Business>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_rejects_duplicate_cnpj()
    {
        _repository.Setup(x => x.ExistsByCnpjAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateBusinessCommand("Acme SA", "Acme", "11.222.333/0001-81");
        var act = () => CreateHandler().Handle(command, default);

        await act.Should().ThrowAsync<DomainException>();
        _repository.Verify(x => x.AddAsync(It.IsAny<Business>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
