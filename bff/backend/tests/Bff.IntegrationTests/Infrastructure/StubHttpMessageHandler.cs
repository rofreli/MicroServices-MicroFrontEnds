namespace Bff.IntegrationTests.Infrastructure;

/// <summary>
/// Fake HTTP transport that lets a test script downstream domain responses without a
/// live server. The responder is resolved per-request so a test can set it after the
/// factory is built.
/// </summary>
public class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

    public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        => _responder = responder;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(_responder(request));
}
