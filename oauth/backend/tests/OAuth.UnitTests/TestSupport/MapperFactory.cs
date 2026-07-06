using AutoMapper;
using OAuth.Application.Mappings;

namespace OAuth.UnitTests.TestSupport;

/// <summary>Builds a real AutoMapper instance from the Application mapping profiles.</summary>
public static class MapperFactory
{
    public static IMapper Create()
    {
        var config = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(UserMappingProfile).Assembly));
        return config.CreateMapper();
    }
}
