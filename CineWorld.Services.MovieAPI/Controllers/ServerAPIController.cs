using AutoMapper;
using CineWorld.Services.MovieAPI.APIFeatures;
using CineWorld.Services.MovieAPI.Exceptions;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineWorld.Services.MovieAPI.Controllers
{
  /// <summary>
  /// Controller for managing servers. Only accessible by Admin role.
  /// </summary>
  [Route("api/servers")]
  [ApiController]
  [Authorize(Roles = SD.AdminRole)]
  public class ServerAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerAPIController"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of work to interact with the database.</param>
    /// <param name="mapper">Mapper to map between entity and DTO.</param>
    public ServerAPIController(IUnitOfWork unitOfWork, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
    }

    /// <summary>
    /// Gets a list of all servers with pagination and filtering options.
    /// </summary>
    /// <param name="queryParameters">The query parameters for filtering and pagination.</param>
    /// <returns>A ResponseDto containing the list of servers with pagination details.</returns>
    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] ServerQueryParameters queryParameters)
    {
      var query = ServerFeatures.Build(queryParameters);
      IEnumerable<Server> servers = await _unitOfWork.Server.GetAllAsync(query);

      _response.Result = _mapper.Map<IEnumerable<ServerDto>>(servers);

      int totalItems = await _unitOfWork.Server.CountAsync();
      _response.Pagination = new PaginationDto
      {
        TotalItems = totalItems,
        TotalItemsPerPage = queryParameters.PageSize,
        CurrentPage = queryParameters.PageNumber,
        TotalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize)
      };

      return Ok(_response);
    }

    /// <summary>
    /// Gets a specific server by its ID.
    /// </summary>
    /// <param name="id">The ID of the server to retrieve.</param>
    /// <returns>A ResponseDto containing the server details.</returns>
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

    /// <summary>
    /// Creates a new server.
    /// </summary>
    /// <param name="serverDto">The data transfer object containing server information.</param>
    /// <returns>A ResponseDto containing the newly created server.</returns>
    [HttpPost]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] ServerDto serverDto)
    {
      Server server = _mapper.Map<Server>(serverDto);

      await _unitOfWork.Server.AddAsync(server);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<ServerDto>(server);

      return Created(string.Empty, _response);
    }

    /// <summary>
    /// Updates an existing server.
    /// </summary>
    /// <param name="serverDto">The data transfer object containing updated server information.</param>
    /// <returns>A ResponseDto containing the updated server.</returns>
    [HttpPut]
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

    /// <summary>
    /// Deletes a server by its ID.
    /// </summary>
    /// <param name="id">The ID of the server to delete.</param>
    /// <returns>A ResponseDto indicating the result of the deletion.</returns>
    [HttpDelete]
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
