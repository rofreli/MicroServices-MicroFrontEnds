using MediatR;

namespace BusinessUnits.Application.Commands.DeleteBusinessUnit;

public record DeleteBusinessUnitCommand(string Id) : IRequest;
