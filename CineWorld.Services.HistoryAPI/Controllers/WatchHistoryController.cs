using AutoMapper;
using CineWorld.Services.HistoryAPI.Data;
using CineWorld.Services.HistoryAPI.Models;
using CineWorld.Services.HistoryAPI.Models.Dtos;
using CineWorld.Services.HistoryAPI.Utilities;
using Mango.Services.HistoryAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineWorld.Services.HistoryAPI.Controllers
{
    [Route("api/watchhistories")]
    [ApiController]
    public class WatchHistoryController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        public WatchHistoryController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public ActionResult<ResponseDto> Get(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] string userId)
        {
            if (fromDate > toDate)
            {
                return BadRequest("fromDate must be earlier than toDate.");
            }
            try
            {
                IEnumerable<WatchHistory> objList = _db.watchHistories.Where(p => p.UserId == userId && p.LastWatched >= fromDate && p.LastWatched <= toDate).ToList();
                var lastWatch = objList.GroupBy(p => p.EpisodeId).Select(g => g.OrderByDescending(p => p.LastWatched).FirstOrDefault()).ToList();
                _response.Result = _mapper.Map<IEnumerable<WatchHistoryDto>>(lastWatch);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }



        [HttpPost]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public ResponseDto Post([FromBody] WatchHistoryDto watchHistoryDto)
        {
            try
            {
                WatchHistory obj = _mapper.Map<WatchHistory>(watchHistoryDto);
                var existingHistories = _db.watchHistories.Where(p => p.EpisodeId == watchHistoryDto.EpisodeId).ToList();
                if (existingHistories.Any())
                {
                    _db.watchHistories.RemoveRange(existingHistories);
                }
                _db.watchHistories.Add(obj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<WatchHistoryDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpPut]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]

        public ResponseDto Put([FromBody] WatchHistoryDto watchHistoryDto)
        {
            try
            {
                WatchHistory obj = _mapper.Map<WatchHistory>(watchHistoryDto);
                _db.watchHistories.Update(obj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<WatchHistoryDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]

        public ResponseDto Delete(int id)
        {
            try
            {
                WatchHistory obj = _db.watchHistories.First(u => u.Id == id);
                _db.watchHistories.Remove(obj);
                _db.SaveChanges();
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
