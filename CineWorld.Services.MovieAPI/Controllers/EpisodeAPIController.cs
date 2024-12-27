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
  [Route("api/episodes")]
  [ApiController]
  public class EpisodeAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;

    public EpisodeAPIController(IMapper mapper, IUnitOfWork unitOfWork)
    {
      _mapper = mapper;
      _response = new ResponseDto();
      _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Gets a list of episodes with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="queryParameters">Query parameters for filtering, sorting, and pagination.</param>
    /// <returns>A paginated list of episodes.</returns>
    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] EpisodeQueryParameters queryParameters)
    {
      var query = EpisodeFeatures.Build(queryParameters);

      bool isAdmin = User.IsInRole(SD.AdminRole);
      if (!isAdmin)
      {
        query.Filters.Add(c => c.Status == true);
      }

      IEnumerable<Episode> episodes = await _unitOfWork.Episode.GetAllAsync(query);

      int totalItems = await _unitOfWork.Episode.CountAsync(query);
      _response.Pagination = new PaginationDto
      {
        TotalItems = totalItems,
        TotalItemsPerPage = queryParameters.PageSize,
        CurrentPage = queryParameters.PageNumber,
        TotalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize)
      };

      _response.Result = _mapper.Map<IEnumerable<EpisodeDto>>(episodes);

      return Ok(_response);
    }

    /// <summary>
    /// Gets an episode by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the episode.</param>
    /// <returns>An episode object with its details.</returns>
    /// <response code="404">Episode not found or not accessible due to inactive membership.</response>
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      bool isAdmin = User.IsInRole(SD.AdminRole);

      string? expirationString = User.Claims.FirstOrDefault(c => c.Type == "MembershipExpiration")?.Value;
      DateTime? membershipExpiration = null;

      if (!string.IsNullOrEmpty(expirationString) && DateTime.TryParse(expirationString, null, System.Globalization.DateTimeStyles.RoundtripKind, out var parsedDate))
      {
        membershipExpiration = parsedDate;
      }

      Episode episode;
      if (isAdmin || (membershipExpiration ?? DateTime.MinValue) > DateTime.UtcNow)
      {
        episode = await _unitOfWork.Episode.GetAsync(c => c.EpisodeId == id, includeProperties: "Servers");
      }
      else
      {
        episode = await _unitOfWork.Episode.GetAsync(c => c.EpisodeId == id && c.IsFree == true, includeProperties: "Servers");
      }

      if (episode == null)
      {
        if (!isAdmin && (membershipExpiration ?? DateTime.MinValue) <= DateTime.UtcNow)
        {
          throw new NotFoundException($"Episode with ID: {id} not found or not accessible due to inactive membership.");
        }
        else
        {
          throw new NotFoundException($"Episode with ID: {id} not found.");
        }
      }

      _response.Result = _mapper.Map<EpisodeDetailsDto>(episode);
      return Ok(_response);
    }

    [HttpPost]
    [Route("GetEpsiodesInfor")]
    public async Task<ActionResult<ResponseDto>> GetEpsiodesInfor([FromBody] IdsRequestDto model)
    {
      var episodes = await _unitOfWork.Episode.GetsAsync(c => model.Ids.Contains(c.EpisodeId), includeProperties: "Movie") ;

      _response.Result = episodes;
      return Ok(_response);
    }

    /// <summary>
    /// Creates a new episode associated with a movie.
    /// </summary>
    /// <param name="episodeDto">The data to create the episode.</param>
    /// <returns>The created episode object.</returns>
    /// <response code="201">Episode created successfully.</response>
    /// <response code="404">Movie not found for the provided MovieId.</response>
    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] EpisodeDto episodeDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var movie = await _unitOfWork.Movie.GetAsync(c => c.MovieId == episodeDto.MovieId, null, true);

      if (movie == null)
      {
        throw new NotFoundException($"Movie with ID: {episodeDto.MovieId} not found.");
      }

      movie.EpisodeTotal++;

      Episode episode = _mapper.Map<Episode>(episodeDto);

      await _unitOfWork.Episode.AddAsync(episode);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<EpisodeDto>(episode);

      return Created(string.Empty, _response);
    }

    /// <summary>
    /// Updates an existing episode.
    /// </summary>
    /// <param name="episodeDto">The data to update the episode.</param>
    /// <returns>The updated episode object.</returns>
    /// <response code="200">Episode updated successfully.</response>
    /// <response code="404">Episode or movie not found for the provided ID.</response>
    [HttpPut]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] EpisodeDto episodeDto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var movie = await _unitOfWork.Movie.GetAsync(c => c.MovieId == episodeDto.MovieId);

      if (movie == null)
      {
        throw new NotFoundException($"Movie with ID: {episodeDto.MovieId} not found.");
      }

      Episode episodeFromDb = await _unitOfWork.Episode.GetAsync(c => c.EpisodeId == episodeDto.EpisodeId);
      if (episodeFromDb == null)
      {
        throw new NotFoundException($"Episode with ID: {episodeDto.EpisodeId} not found.");
      }
      _mapper.Map(episodeDto, episodeFromDb);

      episodeFromDb.UpdatedDate = DateTime.UtcNow;

      await _unitOfWork.Episode.UpdateAsync(episodeFromDb);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<EpisodeDto>(episodeFromDb);

      return Ok(_response);
    }

    /// <summary>
    /// Deletes an episode by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the episode to be deleted.</param>
    /// <returns>No content if deletion is successful.</returns>
    /// <response code="404">Episode not found for the provided ID.</response>
    [HttpDelete]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var episode = await _unitOfWork.Episode.GetAsync(c => c.EpisodeId == id);
      if (episode == null)
      {
        throw new NotFoundException($"Episode with ID: {id} not found.");
      }

      var movie = await _unitOfWork.Movie.GetAsync(c => c.MovieId == episode.MovieId, null, true);
      movie.EpisodeTotal--;

      await _unitOfWork.Episode.RemoveAsync(episode);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }
  }
}
