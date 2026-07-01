using AutoMapper;
using BusinessUnits.Application.DTOs;
using BusinessUnits.Domain.Entities;

namespace BusinessUnits.Application.Mappings;

public class BusinessMappingProfile : Profile
{
    public BusinessMappingProfile()
    {
        CreateMap<Business, BusinessDto>()
            .ConstructUsing(src => new BusinessDto(
                src.Id, src.RazaoSocial, src.NomeFantasia,
                src.Cnpj.Value, src.IsActive, src.CreatedAt, src.UpdatedAt));

        CreateMap<Business, BusinessSummaryDto>()
            .ConstructUsing(src => new BusinessSummaryDto(
                src.Id, src.RazaoSocial, src.NomeFantasia,
                src.Cnpj.Value, src.IsActive, src.CreatedAt));
    }
}
