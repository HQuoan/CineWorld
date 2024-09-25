using AutoMapper;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;


namespace CineWorld.Services.MovieAPI
{
  public class MappingConfig
  {
    public static MapperConfiguration RegisterMaps()
    {
      var mappingConfig = new MapperConfiguration(config =>
      {
        config.CreateMap<Category, CategoryDto>().ReverseMap();
        config.CreateMap<Country, CountryDto>().ReverseMap();
        config.CreateMap<Series, SeriesDto>().ReverseMap();

        config.CreateMap<Movie, MovieDto>().ReverseMap();
        config.CreateMap<Movie, DetailsMovieDto>();

        config.CreateMap<Genre, GenreDto>().ReverseMap();
      });

      return mappingConfig;
    }
  }
}
