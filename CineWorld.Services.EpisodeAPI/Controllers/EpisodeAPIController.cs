using AutoMapper;
using CineWorld.Services.EpisodeAPI.Exceptions;
using CineWorld.Services.EpisodeAPI.Models;
using CineWorld.Services.EpisodeAPI.Models.Dtos;
using CineWorld.Services.EpisodeAPI.Repositories.IRepositories;
using CineWorld.Services.EpisodeAPI.Services.IService;
using CineWorld.Services.EpisodeAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace CineWorld.Services.EpisodeAPI.Controllers
{
  [Route("api/episodes")]
  [ApiController]
  public class EpisodeAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;

    private readonly IMovieService _movieService;

    public EpisodeAPIController(IMapper mapper, IUtil util, IUnitOfWork unitOfWork, IMovieService movieService)
    {
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
      _unitOfWork = unitOfWork;
      _movieService = movieService;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
      IEnumerable<Episode> episodes = await _unitOfWork.Episode.GetAllAsync();
      _response.TotalItems = episodes.Count();
      _response.Result = _mapper.Map<IEnumerable<EpisodeDto>>(episodes);

      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var episode = await _unitOfWork.Episode.GetAsync(c => c.EpisodeId == id);
      if (episode == null)
      {
        throw new NotFoundException($"Episode with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<EpisodeDto>(episode);
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

      var movie = await _movieService.GetMovie(episodeDto.MovieId);
      if(movie == null)
      {
        throw new NotFoundException($"Movie with ID: {episodeDto.MovieId} not found.");
      }

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
      Episode episode = _mapper.Map<Episode>(episodeDto);

      Episode cateFromDb = await _unitOfWork.Episode.GetAsync(c => c.EpisodeId == episodeDto.EpisodeId);
      if (cateFromDb == null)
      {
        throw new NotFoundException($"Episode with ID: {episodeDto.EpisodeId} not found.");
      }

      episode.UpdatedDate = DateTime.UtcNow;

      await _unitOfWork.Episode.UpdateAsync(episode);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<EpisodeDto>(episode);

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

      await _unitOfWork.Episode.RemoveAsync(episode);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }

  }
}
