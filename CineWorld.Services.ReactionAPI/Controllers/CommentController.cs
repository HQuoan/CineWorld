using CineWorld.Services.ReactionAPI.Constants;
using CineWorld.Services.ReactionAPI.Models.Dtos;
using CineWorld.Services.ReactionAPI.Models.Dtos.Comment;
using CineWorld.Services.ReactionAPI.Models.ReqParams;
using CineWorld.Services.ReactionAPI.Services;
using CineWorld.Services.ReactionAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CineWorld.Services.ReactionAPI.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private ResponseDTO response;
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
            response = new ResponseDTO();
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public async Task<ActionResult<ResponseDTO>> AddWatchHistory([FromBody] CreateCommentDTO commentDto)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                response.IsSuccess = await _commentService.AddCommentAsync(userId, commentDto);
                response.Result = commentDto;
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
        public async Task<ActionResult<ResponseDTO>> DeleteComment(int commentId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                string role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                response.IsSuccess = await _commentService.DeleteCommentAsync(role, userId, commentId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }
        [HttpPut]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public async Task<ActionResult<ResponseDTO>> UpdateComment([FromBody] CreateCommentDTO commentDto)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                string role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                response.IsSuccess = await _commentService.UpdateCommentAsync(role, userId, commentDto);
                response.Result = commentDto;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }
        [HttpGet]
        public async Task<ActionResult<ResponseDTO>> GetComments([FromQuery] int movieId, [FromQuery] CommentParam? reqParams)
        {
            try
            {
                response.Result = await _commentService.GetCommentByMovieIdAsync(movieId, reqParams);
                response.IsSuccess = true;
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
