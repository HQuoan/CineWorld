using AutoMapper;
using CineWorld.Services.MovieAPI.Exceptions;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CineWorld.Services.MovieAPI.Controllers
{
  [Route("api/countries")]
  [ApiController]
  public class CountryAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;

    public CountryAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
      IEnumerable<Country> countries = await _unitOfWork.Country.GetAllAsync(new QueryParameters<Country>());
      _response.Result = _mapper.Map<IEnumerable<CountryDto>>(countries);

      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var country = await _unitOfWork.Country.GetAsync(c => c.CountryId == id);
      if(country == null)
      {
        throw new NotFoundException($"Country with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<CountryDto>(country);
      return Ok(_response);
    }

    [HttpGet]
    [Route("{id:int}/movies")]
    public async Task<ActionResult<ResponseDto>> GetWithMovies(int id)
    {
      var country = await _unitOfWork.Country.GetAsync(c => c.CountryId == id, includeProperties: "Movies");
      if (country == null)
      {
        throw new NotFoundException($"Country with ID: {id} not found.");
      }

      // Remove movie with status = false
      _util.FilterMoviesByUserRole(country);
      
      _response.Result = _mapper.Map<CountryMovieDto>(country);
      return Ok(_response);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] CountryDto countryDto)
    {
      Country country = _mapper.Map<Country>(countryDto);
      await _unitOfWork.Country.AddAsync(country);
      await _unitOfWork.SaveAsync();
      _response.Result = _mapper.Map<CountryDto>(country);

      return Created(string.Empty, _response);
    }

    [HttpPut]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] CountryDto countryDto)
    {
      Country country = _mapper.Map<Country>(countryDto);
      await _unitOfWork.Country.UpdateAsync(country);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<CountryDto>(country);

      return Ok(_response);
    }

    [HttpDelete]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var country = await _unitOfWork.Country.GetAsync(c => c.CountryId == id);
      if(country == null)
      {
        throw new NotFoundException($"Country with ID: {id} not found.");
      }

      await _unitOfWork.Country.RemoveAsync(country);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }

  }
}
