using CineWorld.Services.ReactionAPI.Constants;
using CineWorld.Services.ReactionAPI.Models.Dtos;
using CineWorld.Services.ReactionAPI.Models.Dtos.WatchHistory;
using CineWorld.Services.ReactionAPI.Models.ReqParams;
using CineWorld.Services.ReactionAPI.Services;
using CineWorld.Services.ReactionAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CineWorld.Services.ReactionAPI.Controllers
{
    [Route("api/watch_histories")]
    [ApiController]
    public class WatchHistoryController : ControllerBase
    {
        private ResponseDTO response;
        private readonly IWatchHistoryService _watchHistoryService;
        public WatchHistoryController(IWatchHistoryService watchHistoryService)
        {
            _watchHistoryService = watchHistoryService;
            response = new ResponseDTO();
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public async Task<ActionResult<ResponseDTO>> AddWatchHistory([FromBody] WatchHistoryDTO watchHistoryDto)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                response.IsSuccess = await _watchHistoryService.AddWatchHistoryAsync(userId, watchHistoryDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }

        }
        [HttpDelete("{watchHistoryId}")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public async Task<ActionResult<ResponseDTO>> RemoveWatchHistory(int watchHistoryId)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
               response.IsSuccess = await _watchHistoryService.DeleteWatchHistoryAsync(userId, watchHistoryId);
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
        public async Task<ActionResult<ResponseDTO>> RemoveAllWatchHistories()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                response.IsSuccess = await _watchHistoryService.DeleteAllWatchHistoriesAsync(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }
        [HttpGet("GetWatchHistories")]
        public async Task<ActionResult<ResponseDTO>> Get([FromQuery] WatchHistoryParam? reqParams)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                response.IsSuccess = true;
                response.Result = await _watchHistoryService.GetHistoryByUserId(userId, reqParams);
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
