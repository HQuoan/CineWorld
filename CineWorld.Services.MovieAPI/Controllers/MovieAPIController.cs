using AutoMapper;
using CineWorld.Services.MovieAPI.APIFeatures;
using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Exceptions;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.MovieAPI.Controllers
{
  [Route("api/movies")]
  [ApiController]
  public class MovieAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly AppDbContext _db;
    private readonly IUtil _util;

    public MovieAPIController(IUnitOfWork unitOfWork, IMapper mapper, AppDbContext db, IUtil util )
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _db = db;
      _util = util;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] MovieQueryParameters? queryParameters)
    {
      IEnumerable<Movie> movies;
      //if (_util.IsInRoles(new string[] { "ADMIN" }))
      //{
      //  movies = await _unitOfWork.Movie.GetAllAsync(includeProperties: "Category,Country,Series,MovieGenres.Genre");
      //}
      //else
      //{
      //  movies = await _unitOfWork.Movie.GetAllAsync(c => c.Status == true, includeProperties: "Category,Country,Series,MovieGenres.Genre");
      //}

      var query = MovieFeatures.Build(queryParameters);
      query.IncludeProperties = "Category,Country,Series,MovieGenres.Genre";
      movies = await _unitOfWork.Movie.GetAllAsync(query);



      _response.Result = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
      _response.TotalItems = movies.Count();

      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      Movie movie;
      if (_util.IsInRoles(new string[] { "ADMIN" }))
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

      _response.Result = _mapper.Map<MovieDetailsDto>(movie);
      return Ok(_response);
    }

    [HttpPost]
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

      await _unitOfWork.Movie.AddAsync(movie);
      await _unitOfWork.SaveAsync();
      _response.Result = _mapper.Map<MovieDto>(movie);

      return Created(string.Empty, _response);
    }

    [HttpPut]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] MovieDto movieDto)
    {
      var movieFromDb = await _unitOfWork.Movie.GetAsync(m => m.MovieId == movieDto.MovieId, includeProperties: "MovieGenres", tracked: true);

      // Xóa các thể loại hiện tại từ cơ sở dữ liệu
      movieFromDb.MovieGenres.Clear();
      await _unitOfWork.SaveAsync();

      // Cập nhật các thuộc tính của movieFromDb từ movieDto
      _mapper.Map(movieDto, movieFromDb);

      foreach (var genreId in movieDto.GenreIds)
      {
        if (genreId > 0)
        {
          movieFromDb.MovieGenres.Add(new MovieGenre { GenreId = genreId });
        }
      }

      await _unitOfWork.Movie.UpdateAsync(movieFromDb);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<MovieDto>(movieFromDb);

      return Ok(_response);
    }

    [HttpDelete]
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
