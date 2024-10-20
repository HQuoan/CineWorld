using AutoMapper;
using CineWorld.Services.AuthAPI.Models;
using CineWorld.Services.AuthAPI.Models.Dto;



namespace CineWorld.Services.AuthAPI
{
  public class MappingConfig
  {
    public static MapperConfiguration RegisterMaps()
    {
      var mappingConfig = new MapperConfiguration(config =>
      {
        config.CreateMap<ApplicationUser, UserDto>();
      
      });

      return mappingConfig;
    }
  }
}
