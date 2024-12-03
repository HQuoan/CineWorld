using AutoMapper;
using CineWorld.Services.ReactionAPI.Data;
using CineWorld.Services.ReactionAPI.Models;
using CineWorld.Services.ReactionAPI.Models.Dtos;
using CineWorld.Services.ReactionAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Route("CheckFavorite/{movieId:int}")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public ResponseDto CheckFavorite(int movieId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                bool isFavorited = _db.UserFavorites.Any(p => p.MovieId == movieId && p.UserId == userId);

                response.IsSuccess = true;
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
        [Route("AddFavorite")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public ResponseDto AddFavorite([FromBody] UserFavoritesDto favoritesDto)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value; //Không cần gửi userID từ FE
                favoritesDto.UserId = userId; 

                UserFavorites obj = _mapper.Map<UserFavorites>(favoritesDto);
                _db.UserFavorites.Add(obj);
                _db.SaveChanges();

                response.IsSuccess = true;
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
        [Route("RemoveFavorite/{movieId:int}")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public ResponseDto RemoveFavorite(int movieId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var favorite = _db.UserFavorites.FirstOrDefault(p => p.MovieId == movieId && p.UserId == userId);

                if (favorite != null)
                {
                    _db.UserFavorites.Remove(favorite);
                    _db.SaveChanges();
                    response.IsSuccess = true;
                    response.Message = "Đã xóa khỏi danh sách yêu thích.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Phim không có trong danh sách yêu thích.";
                }
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
