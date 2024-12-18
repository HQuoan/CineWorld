using AutoMapper;
using CineWorld.Services.ViewAPI.Models;
using CineWorld.Services.ViewAPI.Models.Dtos;


namespace CineWorld.Services.ViewAPI
{
  public class MappingConfig
  {
    public static MapperConfiguration RegisterMaps()
    {
      var mappingConfig = new MapperConfiguration(config =>
      {
        config.CreateMap<View, ViewDto>().ReverseMap();
      
      });

      return mappingConfig;
    }
  }
}
