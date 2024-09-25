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
  [Route("api/genres")]
  [ApiController]
  [ExceptionHandling]
  public class GenreAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;

    public GenreAPIController(IUnitOfWork unitOfWork, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
      IEnumerable<Genre> genres = await _unitOfWork.Genre.GetAllAsync();
      _response.Result = _mapper.Map<IEnumerable<GenreDto>>(genres);

      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var genre = await _unitOfWork.Genre.GetAsync(c => c.GenreId == id);
      if(genre == null)
      {
        throw new NotFoundException($"Genre with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<GenreDto>(genre);
      return Ok(_response);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] GenreDto genreDto)
    {
      Genre genre = _mapper.Map<Genre>(genreDto);
      await _unitOfWork.Genre.AddAsync(genre);
      await _unitOfWork.SaveAsync();
      _response.Result = _mapper.Map<GenreDto>(genre);

      return Created(string.Empty, _response);
    }

    [HttpPut]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] GenreDto genreDto)
    {
      Genre genre = _mapper.Map<Genre>(genreDto);
      await _unitOfWork.Genre.UpdateAsync(genre);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<GenreDto>(genre);

      return Ok(_response);
    }

    [HttpDelete]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var genre = await _unitOfWork.Genre.GetAsync(c => c.GenreId == id);
      if(genre == null)
      {
        throw new NotFoundException($"Genre with ID: {id} not found.");
      }

      await _unitOfWork.Genre.RemoveAsync(genre);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }

  }
}
