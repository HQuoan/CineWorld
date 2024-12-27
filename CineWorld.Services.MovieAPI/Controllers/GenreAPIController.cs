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
  /// Handles all API requests related to Genre resources, including retrieval, creation, 
  /// updates, and deletions of genres, as well as retrieving movies associated with a genre.
  /// </summary>
  [Route("api/genres")]
  [ApiController]
  public class GenreAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;

    /// <summary>
    /// Initializes a new instance of the GenreAPIController.
    /// </summary>
    /// <param name="unitOfWork">Unit of work for accessing the data repository.</param>
    /// <param name="mapper">AutoMapper instance for DTO mapping.</param>
    /// <param name="util">Utility service for handling unique constraints and slugs.</param>
    public GenreAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
    }

    /// <summary>
    /// Retrieves a list of genres with support for pagination and filtering.
    /// </summary>
    /// <param name="queryParameters">Query parameters for pagination and filtering.</param>
    /// <returns>A paginated list of genres.</returns>
    /// <response code="200">Returns the list of genres.</response>
    /// <response code="400">If the request parameters are invalid.</response>
    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] GenreQueryParameters queryParameters)
    {
      var query = GenreFeatures.Build(queryParameters);
      IEnumerable<Genre> genres = await _unitOfWork.Genre.GetAllAsync(query);

      _response.Result = _mapper.Map<IEnumerable<GenreDto>>(genres);

      int totalItems = await _unitOfWork.Genre.CountAsync(query);
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
    /// Retrieves a genre by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the genre.</param>
    /// <returns>The genre details.</returns>
    /// <response code="200">Returns the genre details.</response>
    /// <response code="404">If the genre with the given ID is not found.</response>
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var genre = await _unitOfWork.Genre.GetAsync(c => c.GenreId == id);
      if (genre == null)
      {
        throw new NotFoundException($"Genre with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<GenreDto>(genre);
      return Ok(_response);
    }

    /// <summary>
    /// Retrieves a genre by its slug.
    /// </summary>
    /// <param name="slug">The slug of the genre.</param>
    /// <returns>The genre details.</returns>
    /// <response code="200">Returns the genre details.</response>
    /// <response code="404">If the genre with the given slug is not found.</response>
    [HttpGet]
    [Route("{slug}")]
    public async Task<ActionResult<ResponseDto>> Get(string slug)
    {
      var genre = await _unitOfWork.Genre.GetAsync(c => c.Slug == slug);
      if (genre == null)
      {
        throw new NotFoundException($"Genre with Slug: {slug} not found.");
      }

      _response.Result = _mapper.Map<GenreDto>(genre);
      return Ok(_response);
    }

    /// <summary>
    /// Retrieves a list of movies associated with a genre by its ID.
    /// </summary>
    /// <param name="id">The ID of the genre.</param>
    /// <param name="queryParameters">Query parameters for pagination and filtering movies.</param>
    /// <returns>A list of movies associated with the genre.</returns>
    /// <response code="200">Returns the list of movies for the specified genre.</response>
    /// <response code="404">If the genre with the given ID is not found.</response>
    [HttpGet]
    [Route("{id:int}/movies")]
    public async Task<ActionResult<ResponseDto>> GetWithMovies(int id, [FromQuery] MovieQueryParameters? queryParameters)
    {
      var genre = await _unitOfWork.Genre.GetAsync(c => c.GenreId == id);
      if (genre == null)
      {
        throw new NotFoundException($"Genre with ID: {id} not found.");
      }

      var query = MovieFeatures.Build(queryParameters);
      query.Filters.Add(m => m.MovieGenres.Any(mg => mg.GenreId == genre.GenreId));

      if (!User.IsInRole(SD.AdminRole))
      {
        query.Filters.Add(c => c.Status == true);
      }

      var movies = await _unitOfWork.Movie.GetAllAsync(query);

      var dto = _mapper.Map<GenreMovieDto>(genre);
      dto.Movies = _mapper.Map<List<MovieDto>>(movies);

      _response.Result = dto;

      return Ok(_response);
    }

    /// <summary>
    /// Creates a new genre.
    /// </summary>
    /// <param name="genreDto">The genre data to create.</param>
    /// <returns>The created genre.</returns>
    /// <response code="201">Returns the created genre.</response>
    /// <response code="400">If the genre data is invalid.</response>
    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] GenreDto genreDto)
    {
      Genre genre = _mapper.Map<Genre>(genreDto);
      genre.Slug = SlugGenerator.GenerateSlug(genre.Name);

      try
      {
        await _unitOfWork.Genre.AddAsync(genre);
        await _unitOfWork.SaveAsync();
      }
      catch (DbUpdateException ex)
      {
        if (_util.IsUniqueConstraintViolation(ex))
        {
          genre.Slug = SlugGenerator.CreateUniqueSlugAsync(genre.Name);
          await _unitOfWork.Genre.AddAsync(genre);
          await _unitOfWork.SaveAsync();
        }
      }

      _response.Result = _mapper.Map<GenreDto>(genre);

      return Created(string.Empty, _response);
    }

    /// <summary>
    /// Updates an existing genre.
    /// </summary>
    /// <param name="genreDto">The genre data to update.</param>
    /// <returns>The updated genre.</returns>
    /// <response code="200">Returns the updated genre.</response>
    /// <response code="400">If the genre data is invalid.</response>
    /// <response code="404">If the genre with the given ID is not found.</response>
    [HttpPut]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] GenreDto genreDto)
    {
      Genre genre = _mapper.Map<Genre>(genreDto);

      Genre genreFromDb = await _unitOfWork.Genre.GetAsync(c => c.GenreId == genreDto.GenreId);
      if (genreFromDb == null)
      {
        throw new NotFoundException($"Genre with ID: {genreDto.GenreId} not found.");
      }

      if (genreFromDb.Name != genre.Name)
      {
        genre.Slug = SlugGenerator.GenerateSlug(genre.Name);
      }

      try
      {
        await _unitOfWork.Genre.UpdateAsync(genre);
        await _unitOfWork.SaveAsync();
      }
      catch (DbUpdateException ex)
      {
        if (_util.IsUniqueConstraintViolation(ex))
        {
          genre.Slug = SlugGenerator.CreateUniqueSlugAsync(genre.Name);
          await _unitOfWork.Genre.UpdateAsync(genre);
          await _unitOfWork.SaveAsync();
        }
      }

      _response.Result = _mapper.Map<GenreDto>(genre);

      return Ok(_response);
    }

    /// <summary>
    /// Deletes a genre by its ID.
    /// </summary>
    /// <param name="id">The ID of the genre to delete.</param>
    /// <returns>No content if successfully deleted.</returns>
    /// <response code="204">If the genre is successfully deleted.</response>
    /// <response code="404">If the genre with the given ID is not found.</response>
    [HttpDelete]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var genre = await _unitOfWork.Genre.GetAsync(c => c.GenreId == id);
      if (genre == null)
      {
        throw new NotFoundException($"Genre with ID: {id} not found.");
      }

      await _unitOfWork.Genre.RemoveAsync(genre);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }
  }

}
