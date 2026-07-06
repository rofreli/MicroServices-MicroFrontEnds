namespace Bff.Application.Common;

/// <summary>Requested aggregate root does not exist in an upstream domain.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entity, string id)
        : base($"{entity} '{id}' was not found.") { }
}

/// <summary>Caller is authenticated but lacks permission over the requested resource.</summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}

/// <summary>A downstream domain API failed or returned an unexpected response.</summary>
public class UpstreamException : Exception
{
    public string Service { get; }

    public UpstreamException(string service, string message, Exception? inner = null)
        : base($"Upstream service '{service}' failed: {message}", inner)
        => Service = service;
}
