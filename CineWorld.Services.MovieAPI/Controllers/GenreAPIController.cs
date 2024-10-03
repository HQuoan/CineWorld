using AutoMapper;
using CineWorld.Services.MovieAPI.APIFeatures;
using CineWorld.Services.MovieAPI.Exceptions;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.Controllers
{
  [Route("api/genres")]
  [ApiController]
  public class GenreAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;

    public GenreAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
      IEnumerable<Genre> genres = await _unitOfWork.Genre.GetAllAsync();
      _response.TotalItems = genres.Count();
      _response.Result = _mapper.Map<IEnumerable<GenreDto>>(genres);

      return Ok(_response);
    }
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

      if (!_util.IsInRoles(new string[] { "ADMIN" }))
      {
        query.Filters.Add(c => c.Status == true);
      }

      var movies = await _unitOfWork.Movie.GetAllAsync(query);

      _response.TotalItems = movies.Count();

      var dto = _mapper.Map<GenreMovieDto>(genre);
      dto.Movies = _mapper.Map<List<MovieDto>>(movies);

      _response.Result = dto;

      return Ok(_response);
    }

   

    [HttpGet]
    [Route("{slug}/movies")]
    public async Task<ActionResult<ResponseDto>> GetWithMovies(string slug, [FromQuery] MovieQueryParameters? queryParameters)
    {
      var genre = await _unitOfWork.Genre.GetAsync(c => c.Slug == slug);
      if (genre == null)
      {
        throw new NotFoundException($"Genre with Slug: {slug} not found.");
      }

      var query = MovieFeatures.Build(queryParameters);
      query.Filters.Add(m => m.MovieGenres.Any(mg => mg.GenreId == genre.GenreId));

      if (!_util.IsInRoles(new string[] { "ADMIN" }))
      {
        query.Filters.Add(c => c.Status == true);
      }

      var movies = await _unitOfWork.Movie.GetAllAsync(query);

      _response.TotalItems = movies.Count();

      var dto = _mapper.Map<GenreMovieDto>(genre);
      dto.Movies = _mapper.Map<List<MovieDto>>(movies);

      _response.Result = dto;

      return Ok(_response);
    }


    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] GenreDto genreDto)
    {

      Genre genre = _mapper.Map<Genre>(genreDto);
      // Generate slug
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

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] GenreDto genreDto)
    {
      Genre genre = _mapper.Map<Genre>(genreDto);

      Genre cateFromDb = await _unitOfWork.Genre.GetAsync(c => c.GenreId == genreDto.GenreId);
      if (cateFromDb == null)
      {
        throw new NotFoundException($"Genre with ID: {genreDto.GenreId} not found.");
      }
      // Generate slug
      if (cateFromDb.Name != genre.Name)
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

    [HttpDelete]
    [Authorize(Roles = "ADMIN")]
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
