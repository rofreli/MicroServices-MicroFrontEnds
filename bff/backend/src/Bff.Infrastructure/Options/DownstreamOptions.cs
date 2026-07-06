namespace Bff.Infrastructure.Options;

/// <summary>Base URLs of the downstream domain APIs the BFF aggregates.</summary>
public class DownstreamOptions
{
    public const string SectionName = "Downstream";

    public string BusinessUnitsApi { get; set; } = "http://localhost:5000";
    public string OAuthApi { get; set; } = "http://localhost:5001";

    /// <summary>Per-request timeout applied to every downstream call.</summary>
    public int TimeoutSeconds { get; set; } = 15;
}
