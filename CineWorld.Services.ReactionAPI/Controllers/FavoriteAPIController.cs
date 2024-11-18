using AutoMapper;
using CineWorld.Services.ReactionAPI.Data;
using CineWorld.Services.ReactionAPI.Models;
using CineWorld.Services.ReactionAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineWorld.Services.ReactionAPI.Controllers
{
    [Route("api/favorite")]
    [ApiController]
    public class FavoriteAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto response;
        private IMapper _mapper;
        public FavoriteAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            response = new ResponseDto();
        }
        [HttpGet]
        [Route("{movieId:int}/{userId}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto IsFavorited(int movieId, string userId)
        {
            try
            {

                bool isFavorited = _db.UserFavorites.Any(p => p.MovieId == movieId && p.UserId == userId);
                response.Result = isFavorited;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;

        }
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] UserFavoritesDto favoritesDto)
        {
            try
            {
                UserFavorites obj = _mapper.Map<UserFavorites>(favoritesDto);
                _db.UserFavorites.Add(obj);
                _db.SaveChanges();






                response.Result = favoritesDto;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        [HttpDelete]
        [Route("{movieId:int}/{userId}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto DeleteFavorite(int movieId, string userId)
        {
            try
            {

                var favorite = _db.UserFavorites
                                  .FirstOrDefault(p => p.MovieId == movieId && p.UserId == userId);


                _db.UserFavorites.Remove(favorite);
                _db.SaveChanges();




            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }


    }
}
