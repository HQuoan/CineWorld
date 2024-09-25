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
        config.CreateMap<Category, CategoryMovieDto>()
        .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src));

        config.CreateMap<Country, CountryDto>().ReverseMap();
        config.CreateMap<Country, CountryMovieDto>()
       .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src));

        config.CreateMap<Series, SeriesDto>().ReverseMap();
        config.CreateMap<Series, SeriesMovieDto>()
       .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src));



        config.CreateMap<Movie, MovieDto>().ReverseMap();
        config.CreateMap<Movie, MovieDetailsDto>()
        .ForMember(dest => dest.Movie, opt => opt.MapFrom(src => src))
        .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
        .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
        .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.Series))
        .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.MovieGenres.Select(mg => new GenreDto
        {
          GenreId = mg.Genre.GenreId,
          Name = mg.Genre.Name
        }).ToList()));


        config.CreateMap<Genre, GenreDto>().ReverseMap();
        config.CreateMap<Genre, GenreMovieDto>()
         .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src))
         .ForMember(dest => dest.Movies, opt => opt.MapFrom(src => src.MovieGenres.Select(mg => new MovieDto
         {
           MovieId = mg.Movie.MovieId,
           CategoryId = mg.Movie.CategoryId,
           CountryId = mg.Movie.CountryId,
           SeriesId = mg.Movie.SeriesId,
           Name = mg.Movie.Name,
           EnglishName = mg.Movie.EnglishName,
           Slug = mg.Movie.Slug,
           EpisodeCount = mg.Movie.EpisodeCount,
           Duration = mg.Movie.Duration,
           Description = mg.Movie.Description,
           ImageUrl = mg.Movie.ImageUrl,
           Trailer = mg.Movie.Trailer,
           Year = mg.Movie.Year,
           IsHot = mg.Movie.IsHot,
           Status = mg.Movie.Status,
         }).ToList()));


      });

      return mappingConfig;
    }
  }
}
