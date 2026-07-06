using Bff.Application.Abstractions;
using Bff.Application.Common;
using Bff.Application.Models.Upstream;
using Bff.Application.Queries.GetBusinessOverview;
using FluentAssertions;
using Moq;
using Xunit;

namespace Bff.UnitTests;

public class GetBusinessOverviewQueryHandlerTests
{
    private const string BusinessId = "biz-1";

    private readonly Mock<IBusinessCatalogApi> _businessCatalog = new();
    private readonly Mock<IUserDirectoryApi> _userDirectory = new();
    private readonly Mock<ICurrentUser> _currentUser = new();

    public GetBusinessOverviewQueryHandlerTests()
    {
        _currentUser.Setup(x => x.CanAccessBusiness(It.IsAny<string>())).Returns(true);
    }

    private GetBusinessOverviewQueryHandler CreateHandler()
        => new(_businessCatalog.Object, _userDirectory.Object, _currentUser.Object);

    private void SetupBusiness(BusinessDetail? business)
        => _businessCatalog
            .Setup(x => x.GetBusinessAsync(BusinessId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(business);

    private static BusinessDetail SampleBusiness() =>
        new(BusinessId, "Acme SA", "Acme", "11.111.111/0001-11", true, DateTime.UtcNow, null);

    [Fact]
    public async Task Handle_throws_Forbidden_when_caller_cannot_access_business()
    {
        _currentUser.Setup(x => x.CanAccessBusiness(BusinessId)).Returns(false);

        var act = () => CreateHandler().Handle(new GetBusinessOverviewQuery(BusinessId), default);

        await act.Should().ThrowAsync<ForbiddenException>();
        // Must short-circuit before hitting any downstream service.
        _businessCatalog.Verify(
            x => x.GetBusinessAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_throws_NotFound_when_business_missing()
    {
        SetupBusiness(null);

        var act = () => CreateHandler().Handle(new GetBusinessOverviewQuery(BusinessId), default);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_composes_business_units_and_team()
    {
        SetupBusiness(SampleBusiness());

        _businessCatalog
            .Setup(x => x.GetBusinessUnitsAsync(BusinessId, 1, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<BusinessUnitSummary>(
                new[]
                {
                    new BusinessUnitSummary("bu-1", BusinessId, "Filial SP", "SP", "22.222.222/0001-22", DateTime.UtcNow),
                },
                TotalCount: 3, Page: 1, PageSize: 20));

        _userDirectory
            .Setup(x => x.GetUsersWithBusinessPermissionAsync(BusinessId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new UserDetail("u-1", "ana@acme.com", "Ana Lima", true, false, new[]
                {
                    new UpstreamPermission(BusinessId, null, "Business", null, "Writer"),
                    new UpstreamPermission(BusinessId, null, "Users", null, "Reader"),
                    // permission on another business must be ignored in the roles list
                    new UpstreamPermission("other-biz", null, "Business", null, "BusinessAdmin"),
                }),
            });

        var result = await CreateHandler().Handle(new GetBusinessOverviewQuery(BusinessId), default);

        result.Business.Id.Should().Be(BusinessId);
        result.BusinessUnitCount.Should().Be(3);
        result.BusinessUnits.Should().ContainSingle().Which.Id.Should().Be("bu-1");

        var member = result.Team.Should().ContainSingle().Subject;
        member.UserId.Should().Be("u-1");
        member.Roles.Should().Equal("Reader", "Writer"); // distinct, sorted, scoped to this business
        member.Roles.Should().NotContain("BusinessAdmin");
    }

    [Fact]
    public async Task Handle_allows_non_admin_with_permission_on_business()
    {
        _currentUser.Setup(x => x.IsSuperAdmin).Returns(false);
        _currentUser.Setup(x => x.CanAccessBusiness(BusinessId)).Returns(true);
        SetupBusiness(SampleBusiness());
        _businessCatalog
            .Setup(x => x.GetBusinessUnitsAsync(BusinessId, 1, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PagedResult<BusinessUnitSummary>.Empty);
        _userDirectory
            .Setup(x => x.GetUsersWithBusinessPermissionAsync(BusinessId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<UserDetail>());

        var result = await CreateHandler().Handle(new GetBusinessOverviewQuery(BusinessId), default);

        result.Business.Id.Should().Be(BusinessId);
        result.Team.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_clamps_page_size_to_upper_bound()
    {
        SetupBusiness(SampleBusiness());
        _businessCatalog
            .Setup(x => x.GetBusinessUnitsAsync(BusinessId, 1, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PagedResult<BusinessUnitSummary>.Empty);
        _userDirectory
            .Setup(x => x.GetUsersWithBusinessPermissionAsync(BusinessId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<UserDetail>());

        await CreateHandler().Handle(new GetBusinessOverviewQuery(BusinessId, BusinessUnitsPageSize: 9999), default);

        _businessCatalog.Verify(
            x => x.GetBusinessUnitsAsync(BusinessId, 1, 100, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
