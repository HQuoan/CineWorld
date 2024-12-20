using AutoMapper;
using CineWorld.Services.ReactionAPI.Models.Dtos.Comment;
using CineWorld.Services.ReactionAPI.Models.Dtos.UserFavorite;
using CineWorld.Services.ReactionAPI.Models.Dtos.UserRate;
using CineWorld.Services.ReactionAPI.Models.Dtos.WatchHistory;
using CineWorld.Services.ReactionAPI.Models.Entities;

namespace CineWorld.Services.ReactionAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<UserRateDTO, UserRate>().ReverseMap();
                config.CreateMap<UserFavoriteDTO, UserFavorite>().ReverseMap();
                config.CreateMap<CreateCommentDTO, Comment>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
                config.CreateMap<Comment, CommentDTO>();
                config.CreateMap<WatchHistoryDTO, WatchHistory>();
                config.CreateMap<WatchHistory, WatchHistoryDTO>()
            .ForMember(dest => dest.MovieName, opt => opt.Ignore());

            });
            return mappingConfig;
        }
    }
}
