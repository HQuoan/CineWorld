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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

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
      IEnumerable<Series> series = await _unitOfWork.Series.GetAllAsync();
      _response.TotalItems = series.Count();
      _response.Result = _mapper.Map<IEnumerable<SeriesDto>>(series);

      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var series = await _unitOfWork.Series.GetAsync(c => c.SeriesId == id);
      if (series == null)
      {
        throw new NotFoundException($"Series with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<SeriesDto>(series);
      return Ok(_response);
    }

    [HttpGet]
    [Route("{slug}")]
    public async Task<ActionResult<ResponseDto>> Get(string slug)
    {
      var series = await _unitOfWork.Series.GetAsync(c => c.Slug == slug);
      if (series == null)
      {
        throw new NotFoundException($"Series with Slug: {slug} not found.");
      }

      _response.Result = _mapper.Map<SeriesDto>(series);
      return Ok(_response);
    }

    [HttpGet]
    [Route("{id:int}/movies")]
    public async Task<ActionResult<ResponseDto>> GetWithMovies(int id, [FromQuery] MovieQueryParameters? queryParameters)
    {
      var series = await _unitOfWork.Series.GetAsync(c => c.SeriesId == id);
      if (series == null)
      {
        throw new NotFoundException($"Series with ID: {id} not found.");
      }

      var query = MovieFeatures.Build(queryParameters);
      query.Filters.Add(c => c.SeriesId == id);

      if (!_util.IsInRoles(new string[] { "ADMIN" }))
      {
        query.Filters.Add(c => c.Status == true);
      }

      series.Movies = await _unitOfWork.Movie.GetAllAsync(query);

      _response.TotalItems = series.Movies.Count();
      _response.Result = _mapper.Map<SeriesMovieDto>(series);

      return Ok(_response);
    }

    [HttpGet]
    [Route("{slug}/movies")]
    public async Task<ActionResult<ResponseDto>> GetWithMovies(string slug, [FromQuery] MovieQueryParameters? queryParameters)
    {
      var series = await _unitOfWork.Series.GetAsync(c => c.Slug == slug);
      if (series == null)
      {
        throw new NotFoundException($"Series with Slug: {slug} not found.");
      }

      var query = MovieFeatures.Build(queryParameters);
      query.Filters.Add(c => c.Slug == slug);

      if (!_util.IsInRoles(new string[] { "ADMIN" }))
      {
        query.Filters.Add(c => c.Status == true);
      }

      series.Movies = await _unitOfWork.Movie.GetAllAsync(query);

      _response.TotalItems = series.Movies.Count();
      _response.Result = _mapper.Map<SeriesMovieDto>(series);

      return Ok(_response);
    }
    [HttpPost]
    [Authorize(Roles ="ADMIN")]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] SeriesDto seriesDto)
    {

      Series series = _mapper.Map<Series>(seriesDto);
      // Generate slug
      series.Slug = SlugGenerator.GenerateSlug(series.Name);

      try
      {
        await _unitOfWork.Series.AddAsync(series);
        await _unitOfWork.SaveAsync();

      }
      catch (DbUpdateException ex)
      {
        if (_util.IsUniqueConstraintViolation(ex))
        {
          series.Slug = SlugGenerator.CreateUniqueSlugAsync(series.Name);
          await _unitOfWork.Series.AddAsync(series);
          await _unitOfWork.SaveAsync();
        }
      }

      _response.Result = _mapper.Map<SeriesDto>(series);

      return Created(string.Empty, _response);
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] SeriesDto seriesDto)
    {
      Series series = _mapper.Map<Series>(seriesDto);

      Series cateFromDb = await _unitOfWork.Series.GetAsync(c => c.SeriesId == seriesDto.SeriesId);
      if (cateFromDb == null)
      {
        throw new NotFoundException($"Series with ID: {seriesDto.SeriesId} not found.");
      }
      // Generate slug
      if (cateFromDb.Name != series.Name)
      {
        series.Slug = SlugGenerator.GenerateSlug(series.Name);
      }

      try
      {
        await _unitOfWork.Series.UpdateAsync(series);
        await _unitOfWork.SaveAsync();

      }
      catch (DbUpdateException ex)
      {
        if (_util.IsUniqueConstraintViolation(ex))
        {
          series.Slug = SlugGenerator.CreateUniqueSlugAsync(series.Name);
          await _unitOfWork.Series.UpdateAsync(series);
          await _unitOfWork.SaveAsync();
        }
      }


      _response.Result = _mapper.Map<SeriesDto>(series);

      return Ok(_response);
    }

    [HttpDelete]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var series = await _unitOfWork.Series.GetAsync(c => c.SeriesId == id);
      if (series == null)
      {
        throw new NotFoundException($"Series with ID: {id} not found.");
      }

      await _unitOfWork.Series.RemoveAsync(series);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }

  }
}
