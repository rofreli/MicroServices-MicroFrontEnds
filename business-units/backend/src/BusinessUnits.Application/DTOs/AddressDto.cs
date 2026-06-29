namespace BusinessUnits.Application.DTOs;

public record AddressDto(
    string Street,
    string Number,
    string? Complement,
    string District,
    string City,
    string State,
    string ZipCode,
    string Country
);

public record AddressInputDto(
    string Street,
    string Number,
    string? Complement,
    string District,
    string City,
    string State,
    string ZipCode,
    string Country = "Brasil"
);
