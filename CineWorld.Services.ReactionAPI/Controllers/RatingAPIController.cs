using CineWorld.Services.ReactionAPI.Constants;
using CineWorld.Services.ReactionAPI.Models.Dtos;
using CineWorld.Services.ReactionAPI.Models.Dtos.UserRate;
using CineWorld.Services.ReactionAPI.Models.Entities;
using CineWorld.Services.ReactionAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CineWorld.Services.ReactionAPI.Controllers
{
    [Route("api/ratings")]
    [ApiController]
    public class RatingAPIController : ControllerBase
    {
        private ResponseDTO _response;
        private readonly IRateService _rateService;
        public RatingAPIController(IRateService rateService)
        {
            _rateService = rateService;
            _response = new ResponseDTO();
        }
        [HttpGet("GetAverageRating/{movieId:int}")]
        public ResponseDTO GetAverageRating(int movieId)
        {
            try
            {
                _response.Result = _rateService.GetAverageRatingAsync(movieId);
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpGet]
        [Route("GetRate/{movieId:int}")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public async Task<ResponseDTO> GetRate(int movieId)
        {
            try
            {
                //double rate = 0;
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                _response.Result = await _rateService.GetRateAsync(movieId, userId);
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;



        }
        [HttpPost]
        [Route("AddRate")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public async Task<ResponseDTO> Post([FromBody] UserRateDTO rate)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
              _response.IsSuccess = await _rateService.AddRateAsync(userId, rate);


            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }
        [HttpPut]
        [Route("UpdateRate")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public async Task<ResponseDTO> Put([FromBody] UserRateDTO rate)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                _response.IsSuccess = await _rateService.UpdateRateAsync(userId, rate);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


    }
}
