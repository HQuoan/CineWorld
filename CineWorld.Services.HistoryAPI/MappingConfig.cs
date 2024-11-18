using AutoMapper;
using CineWorld.Services.HistoryAPI.Models;
using CineWorld.Services.HistoryAPI.Models.Dtos;

namespace CineWorld.Services.HistoryAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps ()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<WatchHistoryDto, WatchHistory>().ReverseMap();
               

            });
            return mappingConfig;
        }
    }
}
