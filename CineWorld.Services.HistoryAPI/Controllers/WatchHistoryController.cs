using AutoMapper;
using CineWorld.Services.HistoryAPI.Data;
using CineWorld.Services.HistoryAPI.Models;
using CineWorld.Services.HistoryAPI.Models.Dtos;
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
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<WatchHistory> objList = _db.watchHistories.ToList();
                _response.Result = _mapper.Map<IEnumerable<WatchHistoryDto>>(objList);
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
        public ResponseDto Post([FromBody] WatchHistoryDto watchHistoryDto)
        {
            try
            {
                WatchHistory obj = _mapper.Map<WatchHistory>(watchHistoryDto);
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
        [Authorize(Roles = "ADMIN")]

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
        [Authorize(Roles = "ADMIN")]

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
