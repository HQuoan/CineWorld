using AutoMapper;
using CineWorld.Services.ReactionAPI.Data;
using CineWorld.Services.ReactionAPI.Models;
using CineWorld.Services.ReactionAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        [Route("{episodeId:int}/{userId}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto GetRate(int episodeId, string userId)
        {
            try
            {
                double rate = 0;
                var rating = _db.UserRates
                    .Where(p => p.EpisodeId == episodeId && p.UserId == userId)
                    .Select(p => p.RatingValue)
                    .FirstOrDefault();
                rate = rating != null ? rating : 0;
                _response.Result = rate;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;



        }
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] UserRatesDto rate)
        {
            try
            {
                UserRates userRates = _mapper.Map<UserRates> (rate);
                _db.UserRates.Add (userRates);
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
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] UserRatesDto rate)
        {
            try
            {
                UserRates userRates = _mapper.Map<UserRates>(rate);
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
