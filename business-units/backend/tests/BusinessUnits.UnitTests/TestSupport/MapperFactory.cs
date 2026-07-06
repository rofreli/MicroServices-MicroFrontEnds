using AutoMapper;
using BusinessUnits.Application.Mappings;

namespace BusinessUnits.UnitTests.TestSupport;

/// <summary>Builds a real AutoMapper instance from the Application mapping profiles.</summary>
public static class MapperFactory
{
    public static IMapper Create()
    {
        var config = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(BusinessMappingProfile).Assembly));
        return config.CreateMapper();
    }
}
