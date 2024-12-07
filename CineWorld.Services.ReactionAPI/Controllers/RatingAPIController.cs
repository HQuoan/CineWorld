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
    [Route("api/rate")]
    [ApiController]
    public class RatingAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        public RatingAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }
        [HttpGet("GetAverageRating/{movieId:int}")]
        public ResponseDto GetAverageRating(int movieId)
        {
            try
            {
                var ratings = _db.UserRates
                    .Where(p => p.MovieId == movieId)
                    .Select(p => p.RatingValue)
                    .ToList();

                if (ratings.Count == 0)
                {
                    _response.IsSuccess = true;
                    _response.Result = 0;
                    _response.Message = "Chưa có đánh giá nào cho phim này.";
                }
                else
                {
                    var averageScore = Math.Round(ratings.Average(), 1);
                    _response.IsSuccess = true;
                    _response.Result = averageScore;
                }
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
        public ResponseDto GetRate(int movieId)
        {
            try
            {
                //double rate = 0;
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var rating = _db.UserRates
                    .Where(p => p.MovieId == movieId && p.UserId == userId).FirstOrDefault();
                if (rating == null)
                {
                    _response.IsSuccess = true;
                    _response.Result = null;
                    _response.Message = "Chưa có đánh giá cho phim này";
                }
                
                var ratingDto = _mapper.Map<UserRatesDto>(rating);
                _response.Result = ratingDto;
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
        public ResponseDto Post([FromBody] UserRatesDto rate)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                UserRates userRates = _mapper.Map<UserRates>(rate);
                userRates.UserId = userId;
                _db.UserRates.Add(userRates);
                _db.SaveChanges();
                _response.Result = rate;

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
        public ResponseDto Put([FromBody] UserRatesDto rate)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                UserRates userRates = _mapper.Map<UserRates>(rate);
                userRates.UserId = userId;
                _db.UserRates.Update(userRates);
                _db.SaveChanges();
                _response.Result = rate;

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
