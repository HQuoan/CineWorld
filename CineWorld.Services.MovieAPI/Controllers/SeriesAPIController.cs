using AutoMapper;
using CineWorld.Services.MovieAPI.Exceptions;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace CineWorld.Services.MovieAPI.Controllers
{
  [Route("api/series")]
  [ApiController]
  public class SeriesAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;

    public SeriesAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
      IEnumerable<Series> series = await _unitOfWork.Series.GetAllAsync(new QueryParameters<Series>());
      _response.Result = _mapper.Map<IEnumerable<SeriesDto>>(series);

      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var series = await _unitOfWork.Series.GetAsync(c => c.SeriesId == id);
      if(series == null)
      {
        throw new NotFoundException($"Series with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<SeriesDto>(series);
      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}/movies")]
    public async Task<ActionResult<ResponseDto>> GetWithMovies(int id)
    {
      var series = await _unitOfWork.Series.GetAsync(c => c.SeriesId == id, includeProperties: "Movies");
      if (series == null)
      {
        throw new NotFoundException($"Series with ID: {id} not found.");
      }

      // Remove movie with status = false
      _util.FilterMoviesByUserRole(series);

      _response.Result = _mapper.Map<SeriesMovieDto>(series);
      return Ok(_response);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] SeriesDto seriesDto)
    {
      Series series = _mapper.Map<Series>(seriesDto);
      await _unitOfWork.Series.AddAsync(series);
      await _unitOfWork.SaveAsync();
      _response.Result = _mapper.Map<SeriesDto>(series);

      return Created(string.Empty, _response);
    }

    [HttpPut]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] SeriesDto seriesDto)
    {
      Series series = _mapper.Map<Series>(seriesDto);
      await _unitOfWork.Series.UpdateAsync(series);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<SeriesDto>(series);

      return Ok(_response);
    }

    [HttpDelete]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var series = await _unitOfWork.Series.GetAsync(c => c.SeriesId == id);
      if(series == null)
      {
        throw new NotFoundException($"Series with ID: {id} not found.");
      }

      await _unitOfWork.Series.RemoveAsync(series);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }

  }
}
