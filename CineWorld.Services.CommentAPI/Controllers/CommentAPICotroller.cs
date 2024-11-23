using AutoMapper;
using CineWorld.Services.CommentAPI.Data;
using CineWorld.Services.CommentAPI.Models;
using CineWorld.Services.CommentAPI.Models.Dtos;
using Mango.Services.CommentAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CineWorld.Services.CommentAPI.Controllers
{
    [Route("api/comment")]
    [ApiController]
    //[Authorize]
    public class CommentAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        public CommentAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Comment> objList = _db.Comments.ToList();
                _response.Result = _mapper.Map<IEnumerable<CommentDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                IEnumerable<Comment> obj = _db.Comments.Where(u => u.MovieId == id).ToList();
                _response.Result = _mapper.Map<IEnumerable<CommentDto>>(obj);
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
        public ResponseDto Post([FromBody] CommentDto CommentDto)
        {
            try
            {
                Comment obj = _mapper.Map<Comment>(CommentDto);
                _db.Comments.Add(obj);
                _db.SaveChanges();






                _response.Result = _mapper.Map<CommentDto>(obj);
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

        public ResponseDto Put([FromBody] CommentDto CommentDto)
        {
            try
            {
                Comment obj = _mapper.Map<Comment>(CommentDto);
                _db.Comments.Update(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CommentDto>(obj);
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
                Comment obj = _db.Comments.First(u => u.CommentId == id);
                _db.Comments.Remove(obj);
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

