using AutoMapper;
using CineWorld.Services.CommentAPI.Data;
using CineWorld.Services.CommentAPI.Models;
using CineWorld.Services.CommentAPI.Models.Dto;
using CineWorld.Services.CommentAPI.Models.Dtos;
using CineWorld.Services.CommentAPI.Services.IService;
using CineWorld.Services.CommentAPI.Utilities;
using Mango.Services.CommentAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        private ICommentService _commentService;
        public CommentAPIController(AppDbContext db, IMapper mapper, ICommentService commentService)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
            _commentService = commentService;
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

        [HttpGet("GetCommentByFilm/{id:int}")]

        public async Task<ResponseDto> GetCommentByFilm(int id)
        {
            try
            {
                UserInformation user = new UserInformation();
                IEnumerable<Comment> obj = _db.Comments.Where(u => u.MovieId == id).ToList();
                //_response.Result = _mapper.Map<IEnumerable<CommentDto>>(obj);
                List<CommentDto> result = new List<CommentDto>();
                foreach (Comment cmt in obj)
                {
                    user = await _commentService.GetUserInformationAsync(cmt.UserId);
                    if (user == null)
                    {
                        Console.WriteLine(user + "null rồi");
                    }
                    CommentDto dto = _mapper.Map<CommentDto>(cmt);
                    dto.UserName = user.FullName;
                    result.Add(dto);

                }
                _response.Result = result;
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
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]

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
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]

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

