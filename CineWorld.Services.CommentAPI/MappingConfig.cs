using AutoMapper;
using CineWorld.Services.CommentAPI.Models;
using CineWorld.Services.CommentAPI.Models.Dtos;



namespace CineWorld.Services.CommentAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CommentDto, Comment>().ReverseMap();
                
            });
            return mappingConfig;
        }
    }
}
