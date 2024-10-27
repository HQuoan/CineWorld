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
using Microsoft.VisualBasic;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.Controllers
{
  [Route("api/movies")]
  [ApiController]
  public class MovieAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;

    public MovieAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] MovieQueryParameters? queryParameters)
    {
      if (!_util.IsInRoles(new string[] { "ADMIN" }))
      {
        queryParameters.Status = true;
      }

      var query = MovieFeatures.Build(queryParameters);
      //query.IncludeProperties = "Category,Country,Series,MovieGenres.Genre";


      IEnumerable<Movie> movies = await _unitOfWork.Movie.GetAllAsync(query);

      _response.Result = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
      _response.TotalItems = movies.Count();

      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      Movie movie;
      bool isAdmin = _util.IsInRoles(new string[] { "ADMIN" });
      if (isAdmin)
      {
        movie = await _unitOfWork.Movie.GetAsync(c => c.MovieId == id, includeProperties: "Category,Country,Series,MovieGenres.Genre");
      }
      else
      {
        movie = await _unitOfWork.Movie.GetAsync(c => c.MovieId == id && c.Status == true, includeProperties: "Category,Country,Series,MovieGenres.Genre");
      }

      if (movie == null)
      {
        throw new NotFoundException($"Movie with ID: {id} not found.");
      }

      var query = new QueryParameters<Episode>();
      query.Filters.Add(c => c.MovieId == movie.MovieId);

      if (!isAdmin)
      {
        query.Filters.Add(c => c.Status == true);
      }

      movie.Episodes = await _unitOfWork.Episode.GetAllAsync(query);

      _response.Result = _mapper.Map<MovieDetailsDto>(movie);
      return Ok(_response);
    }

    [HttpGet]
    [Route("{slug}")]
    public async Task<ActionResult<ResponseDto>> Get(string slug)
    {
      Movie movie;
      bool isAdmin = _util.IsInRoles(new string[] { "ADMIN" });
      if (isAdmin)
      {
        movie = await _unitOfWork.Movie.GetAsync(c => c.Slug == slug, includeProperties: "Category,Country,Series,MovieGenres.Genre");
      }
      else
      {
        movie = await _unitOfWork.Movie.GetAsync(c => c.Slug == slug && c.Status == true, includeProperties: "Category,Country,Series,MovieGenres.Genre");
      }

      if (movie == null)
      {
        throw new NotFoundException($"Movie with Slug: {slug} not found.");
      }

      var query = new QueryParameters<Episode>();
      query.Filters.Add(c => c.MovieId == movie.MovieId);

      if (!isAdmin)
      {
        query.Filters.Add(c => c.Status == true);
      }

      movie.Episodes = await _unitOfWork.Episode.GetAllAsync(query);

      _response.Result = _mapper.Map<MovieDetailsDto>(movie);
      return Ok(_response);
    }


    [HttpPost]
    //[Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] MovieDto movieDto)
    {

      Movie movie = _mapper.Map<Movie>(movieDto);

      foreach (var genreId in movieDto.GenreIds)
      {
        if (genreId > 0)
        {
          movie.MovieGenres.Add(new MovieGenre { GenreId = genreId });
        }
      }
      // Generate slug
      movie.Slug = SlugGenerator.GenerateSlug(movie.Name);

      try
      {
        await _unitOfWork.Movie.AddAsync(movie);
        await _unitOfWork.SaveAsync();

      }
      catch (DbUpdateException ex)
      {
        if (_util.IsUniqueConstraintViolation(ex))
        {
          movie.Slug = SlugGenerator.CreateUniqueSlugAsync(movie.Name);
          await _unitOfWork.Movie.AddAsync(movie);
          await _unitOfWork.SaveAsync();
        }
      }

      _response.Result = _mapper.Map<MovieDto>(movie);

      return Created(string.Empty, _response);
    }

    [HttpPut]
    //[Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] MovieDto movieDto)
    {
      var movieFromDb = await _unitOfWork.Movie.GetAsync(m => m.MovieId == movieDto.MovieId, includeProperties: "MovieGenres", tracked: true);
      if (movieFromDb == null)
      {
        throw new NotFoundException($"Movie with ID: {movieDto.MovieId} not found.");
      }

      // Xóa các thể loại hiện tại từ cơ sở dữ liệu
      movieFromDb.MovieGenres.Clear();
      await _unitOfWork.SaveAsync();

      // Generate slug
      if (movieFromDb.Name != movieDto.Name)
      {
        movieDto.Slug = SlugGenerator.GenerateSlug(movieDto.Name);
      }

      // Cập nhật các thuộc tính của movieFromDb từ movieDto
      _mapper.Map(movieDto, movieFromDb);

      foreach (var genreId in movieDto.GenreIds)
      {
        if (genreId > 0)
        {
          movieFromDb.MovieGenres.Add(new MovieGenre { GenreId = genreId });
        }
      }

      movieFromDb.UpdatedDate = DateTime.UtcNow;

      try
      {
        await _unitOfWork.Movie.UpdateAsync(movieFromDb);
        await _unitOfWork.SaveAsync();
      }
      catch (DbUpdateException ex)
      {
        if (_util.IsUniqueConstraintViolation(ex))
        {
          movieFromDb.Slug = SlugGenerator.CreateUniqueSlugAsync(movieFromDb.Name);
          await _unitOfWork.Movie.UpdateAsync(movieFromDb);
          await _unitOfWork.SaveAsync();
        }
      }

      _response.Result = _mapper.Map<MovieDto>(movieFromDb);

      return Ok(_response);
    }

    [HttpDelete]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var movie = await _unitOfWork.Movie.GetAsync(c => c.MovieId == id);
      if (movie == null)
      {
        throw new NotFoundException($"Movie with ID: {id} not found.");
      }

      await _unitOfWork.Movie.RemoveAsync(movie);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }

  }
}
