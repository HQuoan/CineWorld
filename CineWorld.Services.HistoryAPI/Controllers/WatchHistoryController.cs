using AutoMapper;
using CineWorld.Services.HistoryAPI.Data;
using CineWorld.Services.HistoryAPI.Models;
using CineWorld.Services.HistoryAPI.Models.Dtos;
using CineWorld.Services.HistoryAPI.Utilities;
using Mango.Services.HistoryAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [HttpGet("FilterByTime")]
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
        [HttpGet("GetHistoryByUserId")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public ActionResult<ResponseDto> Get()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                IEnumerable<WatchHistory> watchHistories = _db.watchHistories.Where(p => p.UserId == userId).ToList();
                _response.Result = _mapper.Map<IEnumerable<WatchHistoryDto>>(watchHistories);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpPost("PostHistory")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public ResponseDto Post([FromBody] WatchHistoryDto watchHistoryDto)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                WatchHistory obj = _mapper.Map<WatchHistory>(watchHistoryDto);
                obj.UserId = userId;
                var existingHistories = _db.watchHistories.Where(p => p.EpisodeId == watchHistoryDto.EpisodeId && p.MovieId == watchHistoryDto.MovieId).ToList();
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

        [HttpDelete("DeleteHistory")]

        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]

        public ResponseDto Delete(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] string userId)
        {
            try
            {
                IEnumerable<WatchHistory> objList = _db.watchHistories.Where(p => p.UserId == userId && p.LastWatched >= fromDate && p.LastWatched <= toDate).ToList();
                if (objList.Any())
                {
                    _db.watchHistories.RemoveRange(objList);
                }
                _db.SaveChanges();
                _response.Message = $"Xóa lịch sử phim trong khoảng thời gian từ {fromDate} tới {toDate} thành công!";

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
