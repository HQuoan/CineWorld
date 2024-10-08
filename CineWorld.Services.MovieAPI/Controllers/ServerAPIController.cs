using AutoMapper;
using CineWorld.Services.MovieAPI.APIFeatures;
using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Exceptions;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CineWorld.Services.MovieAPI.Controllers
{
  [Route("api/servers")]
  [ApiController]
  public class ServerAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;

    public ServerAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
      IEnumerable<Server> servers = await _unitOfWork.Server.GetAllAsync();
      _response.TotalItems = servers.Count();
      _response.Result = _mapper.Map<IEnumerable<ServerDto>>(servers);

      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var server = await _unitOfWork.Server.GetAsync(c => c.ServerId == id);
      if (server == null)
      {
        throw new NotFoundException($"Server with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<ServerDto>(server);
      return Ok(_response);
    }

    [HttpPost]
    //[Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] ServerDto serverDto)
    {

      Server server = _mapper.Map<Server>(serverDto);

      await _unitOfWork.Server.AddAsync(server);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<ServerDto>(server);

      return Created(string.Empty, _response);
    }

    [HttpPut]
   // [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] ServerDto serverDto)
    {
      Server server = _mapper.Map<Server>(serverDto);

      Server serverFromDb = await _unitOfWork.Server.GetAsync(c => c.ServerId == serverDto.ServerId);
      if (serverFromDb == null)
      {
        throw new NotFoundException($"Server with ID: {serverDto.ServerId} not found.");
      }

      await _unitOfWork.Server.UpdateAsync(server);
      await _unitOfWork.SaveAsync();


      _response.Result = _mapper.Map<ServerDto>(server);

      return Ok(_response);
    }

    [HttpDelete]
   // [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var server = await _unitOfWork.Server.GetAsync(c => c.ServerId == id);
      if (server == null)
      {
        throw new NotFoundException($"Server with ID: {id} not found.");
      }

      await _unitOfWork.Server.RemoveAsync(server);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }

  }
}
