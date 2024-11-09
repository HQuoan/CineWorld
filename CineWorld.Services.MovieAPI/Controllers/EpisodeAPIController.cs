using AutoMapper;
using CineWorld.Services.MovieAPI.Exceptions;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Utilities;
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
    private readonly IUtil _util;


    public EpisodeAPIController(IMapper mapper, IUtil util, IUnitOfWork unitOfWork)
    {
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
      _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {

      var query = new QueryParameters<Episode>();
      
      bool isAdmin = _util.IsInRoles(new string[] { "ADMIN" });
      if (!isAdmin)
      {
        query.Filters.Add(c => c.Status == true);
      }

      IEnumerable<Episode> episodes = await _unitOfWork.Episode.GetAllAsync(query);
      //_response.TotalItems = episodes.Count();
      _response.Result = _mapper.Map<IEnumerable<EpisodeDto>>(episodes);

      return Ok(_response);
    }
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
        episode = await _unitOfWork.Episode.GetAsync(c => c.EpisodeId == id && c.Status == true);
      }

      if (episode == null)
      {
        throw new NotFoundException($"Episode with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<EpisodeDetailsDto>(episode);
      return Ok(_response);
    }

    [HttpPost]
    // [Authorize(Roles ="ADMIN")]
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

    [HttpPut]
    //[Authorize(Roles = "ADMIN")]
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

    [HttpDelete]
    // [Authorize(Roles = "ADMIN")]
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
