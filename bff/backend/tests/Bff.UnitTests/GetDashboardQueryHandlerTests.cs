using Bff.Application.Abstractions;
using Bff.Application.Common;
using Bff.Application.Queries.GetDashboard;
using FluentAssertions;
using Moq;
using Xunit;

namespace Bff.UnitTests;

public class GetDashboardQueryHandlerTests
{
    private readonly Mock<IBusinessCatalogApi> _businessCatalog = new();
    private readonly Mock<IUserDirectoryApi> _userDirectory = new();

    private GetDashboardQueryHandler CreateHandler()
        => new(_businessCatalog.Object, _userDirectory.Object);

    [Fact]
    public async Task Handle_composes_counts_from_both_domains()
    {
        _businessCatalog.Setup(x => x.CountBusinessesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(7);
        _businessCatalog.Setup(x => x.CountBusinessUnitsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(19);
        _userDirectory.Setup(x => x.CountUsersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

        var result = await CreateHandler().Handle(new GetDashboardQuery(), default);

        result.BusinessCount.Should().Be(7);
        result.BusinessUnitCount.Should().Be(19);
        result.UserCount.Should().Be(42);
        result.GeneratedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_queries_all_three_counts_exactly_once()
    {
        _businessCatalog.Setup(x => x.CountBusinessesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _businessCatalog.Setup(x => x.CountBusinessUnitsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _userDirectory.Setup(x => x.CountUsersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await CreateHandler().Handle(new GetDashboardQuery(), default);

        _businessCatalog.Verify(x => x.CountBusinessesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _businessCatalog.Verify(x => x.CountBusinessUnitsAsync(It.IsAny<CancellationToken>()), Times.Once);
        _userDirectory.Verify(x => x.CountUsersAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_propagates_upstream_failure()
    {
        _businessCatalog.Setup(x => x.CountBusinessesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UpstreamException("business-units", "boom"));
        _businessCatalog.Setup(x => x.CountBusinessUnitsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _userDirectory.Setup(x => x.CountUsersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var act = () => CreateHandler().Handle(new GetDashboardQuery(), default);

        await act.Should().ThrowAsync<UpstreamException>();
    }
}
