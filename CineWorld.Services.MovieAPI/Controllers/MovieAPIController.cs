using AutoMapper;
using CineWorld.Services.CouponAPI.Attributes;
using CineWorld.Services.MovieAPI.Exceptions;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CineWorld.Services.MovieAPI.Controllers
{
  [Route("api/movies")]
  [ApiController]
  [ExceptionHandling]
  public class MovieAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;

    public MovieAPIController(IUnitOfWork unitOfWork, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
      IEnumerable<Movie> movies = await _unitOfWork.Movie.GetAllAsync();
      _response.Result = _mapper.Map<IEnumerable<MovieDto>>(movies);

      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var movie = await _unitOfWork.Movie.GetAsync(c => c.MovieId == id, includeProperties: "Category,Country,Series");
      if(movie == null)
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
     
      await _unitOfWork.Movie.AddAsync(movie);
      await _unitOfWork.SaveAsync();
      _response.Result = _mapper.Map<MovieDto>(movie);

      return Created(string.Empty, _response);
    }

    [HttpPut]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] MovieDto movieDto)
    {
      Movie movie = _mapper.Map<Movie>(movieDto);
      await _unitOfWork.Movie.UpdateAsync(movie);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<MovieDto>(movie);

      return Ok(_response);
    }

    [HttpDelete]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var movie = await _unitOfWork.Movie.GetAsync(c => c.MovieId == id);
      if(movie == null)
      {
        throw new NotFoundException($"Movie with ID: {id} not found.");
      }

      await _unitOfWork.Movie.RemoveAsync(movie);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }

  }
}
