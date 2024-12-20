using CineWorld.Services.ReactionAPI.Constants;
using CineWorld.Services.ReactionAPI.Models.Common;
using CineWorld.Services.ReactionAPI.Models.Dtos;
using CineWorld.Services.ReactionAPI.Models.Dtos.UserFavorite;
using CineWorld.Services.ReactionAPI.Models.ReqParams;
using CineWorld.Services.ReactionAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CineWorld.Services.ReactionAPI.Controllers
{
    [Route("api/favorites")]
    [ApiController]
    public class FavoriteAPIController : ControllerBase
    {
        private ResponseDTO response;
        private readonly IFavoriteService _favoriteService;
        public FavoriteAPIController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
            response = new ResponseDTO();
        }
        [HttpGet]
        [Route("{movieId:int}")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public async Task<ActionResult<ResponseDTO>> IsFavorited(int movieId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                response.Result = await _favoriteService.CheckFavoriteAsync(userId, movieId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }

        }
        [HttpPost]

        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public async Task<ActionResult<ResponseDTO>> AddFavorite([FromBody] UserFavoriteDTO favoritesDto)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                response.IsSuccess = await _favoriteService.AddFavoriteAsync(userId, favoritesDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
            
        }
        [HttpDelete]

        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public async Task<ActionResult<ResponseDTO>> RemoveFavorite(UserFavoriteDTO favoritesDto)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                response.IsSuccess = await _favoriteService.RemoveFavoriteAsync(userId, favoritesDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
           
        }
        [HttpGet("GetUserFavorites")]
        public async Task<ActionResult<ResponseDTO>> Get([FromQuery] FavoriteParam? reqParams)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                response.IsSuccess = true;
                response.Result = await _favoriteService.GetFavoriteByUserId(userId, reqParams);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
            
        }



    }
}
