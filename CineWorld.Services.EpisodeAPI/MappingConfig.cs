using AutoMapper;
using CineWorld.Services.EpisodeAPI.Models;
using CineWorld.Services.EpisodeAPI.Models.Dtos;


namespace CineWorld.Services.EpisodeAPI
{
  public class MappingConfig
  {
    public static MapperConfiguration RegisterMaps()
    {
      var mappingConfig = new MapperConfiguration(config =>
      {
        config.CreateMap<Episode, EpisodeDto>();
        config.CreateMap<EpisodeDto, Episode>()
        .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
        .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore());

        config.CreateMap<Episode, EpisodeDetailsDto>()
       .ForMember(dest => dest.Movie, opt => opt.MapFrom(src => src));

        config.CreateMap<Server, ServerDto>().ReverseMap();
      
      });

      return mappingConfig;
    }
  }
}
