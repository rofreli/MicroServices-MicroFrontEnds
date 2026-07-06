using BusinessUnits.Application.Commands.DeleteBusiness;
using BusinessUnits.Domain.Entities;
using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BusinessUnits.UnitTests.Application;

public class DeleteBusinessCommandHandlerTests
{
    private readonly Mock<IBusinessRepository> _businessRepository = new();
    private readonly Mock<IBusinessUnitRepository> _buRepository = new();

    private DeleteBusinessCommandHandler CreateHandler()
        => new(_businessRepository.Object, _buRepository.Object);

    private static Business ExistingBusiness()
        => Business.Create("Acme SA", "Acme", "11.222.333/0001-81");

    [Fact]
    public async Task Handle_throws_when_business_missing()
    {
        _businessRepository.Setup(x => x.GetByIdAsync("biz-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Business?)null);

        var act = () => CreateHandler().Handle(new DeleteBusinessCommand("biz-1"), default);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_blocks_deletion_when_business_has_units()
    {
        _businessRepository.Setup(x => x.GetByIdAsync("biz-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingBusiness());
        _buRepository.Setup(x => x.CountAsync("biz-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var act = () => CreateHandler().Handle(new DeleteBusinessCommand("biz-1"), default);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*still has 3 Business Unit*");
        _businessRepository.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_deletes_when_no_units_remain()
    {
        _businessRepository.Setup(x => x.GetByIdAsync("biz-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExistingBusiness());
        _buRepository.Setup(x => x.CountAsync("biz-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        await CreateHandler().Handle(new DeleteBusinessCommand("biz-1"), default);

        _businessRepository.Verify(x => x.DeleteAsync("biz-1", It.IsAny<CancellationToken>()), Times.Once);
    }
}
