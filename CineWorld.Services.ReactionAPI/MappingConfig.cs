using AutoMapper;
using CineWorld.Services.ReactionAPI.Models;
using CineWorld.Services.ReactionAPI.Models.Dtos;

namespace CineWorld.Services.ReactionAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps ()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<UserRatesDto, UserRates>().ReverseMap();
                config.CreateMap<UserFavoritesDto, UserFavorites>().ReverseMap();

            });
            return mappingConfig;
        }
    }
}
