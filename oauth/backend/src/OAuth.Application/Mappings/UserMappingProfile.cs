using AutoMapper;
using OAuth.Application.DTOs;
using OAuth.Domain.Entities;

namespace OAuth.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ConstructUsing(src => new UserDto(
                src.Id, src.Email.Value, src.FirstName, src.LastName, src.FullName,
                src.IsActive,
                src.Roles,
                src.ExternalProviders.Select(p => p.Provider).ToList(),
                src.CreatedAt, src.UpdatedAt
            ));

        CreateMap<User, UserSummaryDto>()
            .ConstructUsing(src => new UserSummaryDto(
                src.Id, src.Email.Value, src.FullName,
                src.IsActive, src.Roles, src.CreatedAt
            ));
    }
}
