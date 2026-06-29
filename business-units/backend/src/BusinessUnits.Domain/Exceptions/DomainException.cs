namespace BusinessUnits.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entity, string id)
        : base($"{entity} with id '{id}' was not found.") { }
}
