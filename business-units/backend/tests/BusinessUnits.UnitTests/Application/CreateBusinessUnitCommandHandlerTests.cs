using BusinessUnits.Application.Commands.CreateBusinessUnit;
using BusinessUnits.Domain.Entities;
using BusinessUnits.Domain.Exceptions;
using BusinessUnits.Domain.Interfaces;
using BusinessUnits.UnitTests.TestSupport;
using FluentAssertions;
using Moq;
using Xunit;

namespace BusinessUnits.UnitTests.Application;

public class CreateBusinessUnitCommandHandlerTests
{
    private const string ValidCnpj = "44.555.666/0001-81";

    private readonly Mock<IBusinessUnitRepository> _buRepository = new();
    private readonly Mock<IBusinessRepository> _businessRepository = new();

    private CreateBusinessUnitCommandHandler CreateHandler()
        => new(_buRepository.Object, _businessRepository.Object, MapperFactory.Create());

    private CreateBusinessUnitCommand Command(string businessId = "biz-1")
        => new(businessId, "Filial SP", "SP", ValidCnpj, Address: null, Contacts: null);

    [Fact]
    public async Task Handle_rejects_when_parent_business_does_not_exist()
    {
        _businessRepository.Setup(x => x.ExistsByIdAsync("biz-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => CreateHandler().Handle(Command(), default);

        await act.Should().ThrowAsync<NotFoundException>();
        _buRepository.Verify(x => x.AddAsync(It.IsAny<BusinessUnit>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_rejects_duplicate_cnpj()
    {
        _businessRepository.Setup(x => x.ExistsByIdAsync("biz-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _buRepository.Setup(x => x.ExistsByCnpjAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => CreateHandler().Handle(Command(), default);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_creates_unit_under_existing_business()
    {
        _businessRepository.Setup(x => x.ExistsByIdAsync("biz-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _buRepository.Setup(x => x.ExistsByCnpjAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await CreateHandler().Handle(Command(), default);

        result.BusinessId.Should().Be("biz-1");
        result.RazaoSocial.Should().Be("Filial SP");
        _buRepository.Verify(x => x.AddAsync(It.IsAny<BusinessUnit>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
