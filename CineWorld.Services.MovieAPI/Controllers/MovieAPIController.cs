using AutoMapper;
using CineWorld.Services.CouponAPI.Attributes;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CineWorld.Services.MovieAPI.Controllers
{
  [Route("api/movies")]
  [ApiController]
  //[ExceptionHandling]
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
      var movie = await _unitOfWork.Movie.GetAsync(c => c.MovieId == id);
      if(movie == null)
      {

      }

      _response.Result = _mapper.Map<MovieDto>(movie);
      return Ok(_response);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] MovieDto movieDto)
    {
      Movie movie = _mapper.Map<Movie>(movieDto);
      await _unitOfWork.Movie.AddAsync(movie);
      await _unitOfWork.SaveAsync();
      _response.Result = _mapper.Map<MovieDto>(movie);

      return Ok(_response);
    }


  }
}
