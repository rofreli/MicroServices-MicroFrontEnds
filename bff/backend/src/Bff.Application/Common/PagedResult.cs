namespace Bff.Application.Common;

/// <summary>
/// Mirrors the paginated envelope returned by the downstream domain APIs
/// (<c>PaginatedResult&lt;T&gt;</c>). Only the fields the BFF actually consumes are modelled.
/// </summary>
public record PagedResult<T>(
    IReadOnlyList<T> Items,
    long TotalCount,
    int Page,
    int PageSize
)
{
    public static PagedResult<T> Empty { get; } =
        new(Array.Empty<T>(), 0, 1, 0);
}
