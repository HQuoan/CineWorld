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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CineWorld.Services.CommentAPI.Controllers
{
    [Route("api/comment")]
    [ApiController]

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

        [HttpGet("GetCommentByFilmId/{id:int}")]

        public async Task<ResponseDto> GetCommentsByFilmId(int id)
        {
            try
            {
                UserInformation user = new UserInformation();
                IEnumerable<Comment> comments = _db.Comments.Where(u => u.MovieId == id).ToList();

                _response.Result = _mapper.Map<IEnumerable<CommentDto>>(comments);
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
        public async Task<ResponseDto> Post([FromBody] CommentDto CommentDto)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var fullNameClaim = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
            var avatarClaim = User.FindFirst("Avatar")?.Value;
            if (fullNameClaim == null || userId == null)
            {
                _response.Message = "You are not allowed to access data that does not belong to you.";
            }
            try
            {
                Comment obj = _mapper.Map<Comment>(CommentDto);
                obj.Avatar = avatarClaim;
                obj.FullName = fullNameClaim;
                obj.UserId = userId;
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

