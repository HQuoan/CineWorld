using AutoMapper;
using CineWorld.Services.MovieAPI.APIFeatures;
using CineWorld.Services.MovieAPI.Exceptions;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.MovieAPI.Controllers
{
  /// <summary>
  /// Controller for managing Series.
  /// </summary>
  [Route("api/series")]
  [ApiController]
  public class SeriesAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;

    /// <summary>
    /// Initializes a new instance of the SeriesAPIController.
    /// </summary>
    public SeriesAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
    }

    /// <summary>
    /// Gets a list of all series with pagination.
    /// </summary>
    /// <param name="queryParameters">The query parameters to filter and paginate the series list.</param>
    /// <returns>A ResponseDto containing a list of series.</returns>
    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] SeriesQueryParameters queryParameters)
    {
      var query = SeriesFeatures.Build(queryParameters);
      IEnumerable<Series> series = await _unitOfWork.Series.GetAllAsync(query);

      _response.Result = _mapper.Map<IEnumerable<SeriesDto>>(series);

      int totalItems = await _unitOfWork.Series.CountAsync(query);
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
    /// Gets a specific series by its ID.
    /// </summary>
    /// <param name="id">The ID of the series to retrieve.</param>
    /// <returns>A ResponseDto containing the series data.</returns>
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

    /// <summary>
    /// Gets a specific series by its slug.
    /// </summary>
    /// <param name="slug">The slug of the series to retrieve.</param>
    /// <returns>A ResponseDto containing the series data.</returns>
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

    /// <summary>
    /// Gets all movies related to a specific series by its ID.
    /// </summary>
    /// <param name="id">The ID of the series.</param>
    /// <param name="queryParameters">The query parameters for filtering and pagination of movies.</param>
    /// <returns>A ResponseDto containing the series and related movies.</returns>
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

      if (!User.IsInRole(SD.AdminRole))
      {
        query.Filters.Add(c => c.Status == true);
      }

      series.Movies = await _unitOfWork.Movie.GetAllAsync(query);

      _response.Result = _mapper.Map<SeriesMovieDto>(series);

      return Ok(_response);
    }

    /// <summary>
    /// Gets all movies related to a specific series by its slug.
    /// </summary>
    /// <param name="slug">The slug of the series.</param>
    /// <param name="queryParameters">The query parameters for filtering and pagination of movies.</param>
    /// <returns>A ResponseDto containing the series and related movies.</returns>
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
      query.Filters.Add(c => c.SeriesId == series.SeriesId);

      if (!User.IsInRole(SD.AdminRole))
      {
        query.Filters.Add(c => c.Status == true);
      }

      series.Movies = await _unitOfWork.Movie.GetAllAsync(query);

      _response.Result = _mapper.Map<SeriesMovieDto>(series);

      return Ok(_response);
    }

    /// <summary>
    /// Creates a new series.
    /// </summary>
    /// <param name="seriesDto">The data transfer object containing series information.</param>
    /// <returns>A ResponseDto containing the created series.</returns>
    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
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

    /// <summary>
    /// Updates an existing series.
    /// </summary>
    /// <param name="seriesDto">The data transfer object containing updated series information.</param>
    /// <returns>A ResponseDto containing the updated series.</returns>
    [HttpPut]
    [Authorize(Roles = SD.AdminRole)]
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

    /// <summary>
    /// Deletes a series by its ID.
    /// </summary>
    /// <param name="id">The ID of the series to delete.</param>
    /// <returns>A ResponseDto indicating the result of the deletion.</returns>
    [HttpDelete]
    [Authorize(Roles = SD.AdminRole)]
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
