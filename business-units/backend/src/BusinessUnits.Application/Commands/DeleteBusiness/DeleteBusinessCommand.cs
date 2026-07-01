using MediatR;

namespace BusinessUnits.Application.Commands.DeleteBusiness;

public record DeleteBusinessCommand(string Id) : IRequest;
