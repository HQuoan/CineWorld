﻿using AutoMapper;
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

        config.CreateMap<Episode, EpisodeDto>();
        config.CreateMap<EpisodeDto, Episode>();

        config.CreateMap<Episode, EpisodeDetailsDto>()
       .ForMember(dest => dest.Episode, opt => opt.MapFrom(src => src));

        config.CreateMap<Server, ServerDto>().ReverseMap();


        config.CreateMap<Movie, MovieDto>();
        config.CreateMap<MovieDto, Movie>();
        config.CreateMap<Movie, MovieDetailsDto>()
        .ForMember(dest => dest.Movie, opt => opt.MapFrom(src => src))
        .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
        .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
        .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.Series))
        .ForMember(dest => dest.Episodes, opt => opt.MapFrom(src => src.Episodes))
        .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.MovieGenres.Select(mg => new GenreDto
        {
          GenreId = mg.Genre.GenreId,
          Name = mg.Genre.Name,
          Slug = mg.Genre.Slug,
          Status = mg.Genre.Status
        }).ToList()));


        config.CreateMap<Genre, GenreDto>().ReverseMap();
        config.CreateMap<Genre, GenreMovieDto>()
         .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src));
      });

      return mappingConfig;
    }
  }
}
