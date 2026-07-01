using AutoMapper;
using BusinessUnits.Application.DTOs;
using BusinessUnits.Domain.Entities;

namespace BusinessUnits.Application.Mappings;

public class BusinessUnitMappingProfile : Profile
{
    public BusinessUnitMappingProfile()
    {
        CreateMap<BusinessUnit, BusinessUnitDto>()
            .ConstructUsing((src, ctx) => new BusinessUnitDto(
                src.Id,
                src.BusinessId,
                src.RazaoSocial,
                src.NomeFantasia,
                src.Cnpj.Value,
                src.Address is null ? null : ctx.Mapper.Map<AddressDto>(src.Address),
                src.Contacts.Select(ctx.Mapper.Map<ContactDto>).ToList(),
                src.CreatedAt,
                src.UpdatedAt
            ));

        CreateMap<BusinessUnit, BusinessUnitSummaryDto>()
            .ConstructUsing(src => new BusinessUnitSummaryDto(
                src.Id,
                src.BusinessId,
                src.RazaoSocial,
                src.NomeFantasia,
                src.Cnpj.Value,
                src.CreatedAt
            ));

        CreateMap<Address, AddressDto>()
            .ConstructUsing(src => new AddressDto(
                src.Street, src.Number, src.Complement,
                src.District, src.City, src.State, src.ZipCode, src.Country
            ));

        CreateMap<Contact, ContactDto>()
            .ConstructUsing(src => new ContactDto(src.Id, src.Name, src.Email, src.Phone, src.Type));
    }
}
