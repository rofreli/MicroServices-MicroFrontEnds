using AutoMapper;
using OAuth.Application.DTOs;
using OAuth.Domain.Entities;

namespace OAuth.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<Permission, PermissionDto>()
            .ConstructUsing(src => new PermissionDto(
                src.BusinessId, src.BusinessUnitId, src.Module, src.Function, src.Role));

        CreateMap<User, UserDto>()
            .ConstructUsing((src, ctx) => new UserDto(
                src.Id, src.Email.Value, src.FirstName, src.LastName, src.FullName,
                src.IsActive,
                src.IsSuperAdmin,
                src.Permissions.Select(ctx.Mapper.Map<PermissionDto>).ToList(),
                src.ExternalProviders.Select(p => p.Provider).ToList(),
                src.CreatedAt, src.UpdatedAt
            ));

        CreateMap<User, UserSummaryDto>()
            .ConstructUsing(src => new UserSummaryDto(
                src.Id, src.Email.Value, src.FullName,
                src.IsActive, src.IsSuperAdmin, src.CreatedAt
            ));
    }
}
