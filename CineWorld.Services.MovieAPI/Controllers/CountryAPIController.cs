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
      IEnumerable<Country> countries = await _unitOfWork.Country.GetAllAsync();
      _response.TotalItems = countries.Count();
      _response.Result = _mapper.Map<IEnumerable<CountryDto>>(countries);

      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var country = await _unitOfWork.Country.GetAsync(c => c.CountryId == id);
      if (country == null)
      {
        throw new NotFoundException($"Country with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<CountryDto>(country);
      return Ok(_response);
    }

    [HttpGet]
    [Route("{slug}")]
    public async Task<ActionResult<ResponseDto>> Get(string slug)
    {
      var country = await _unitOfWork.Country.GetAsync(c => c.Slug == slug);
      if (country == null)
      {
        throw new NotFoundException($"Country with Slug: {slug} not found.");
      }

      _response.Result = _mapper.Map<CountryDto>(country);
      return Ok(_response);
    }

    [HttpGet]
    [Route("{id:int}/movies")]
    public async Task<ActionResult<ResponseDto>> GetWithMovies(int id, [FromQuery] MovieQueryParameters? queryParameters)
    {
      var country = await _unitOfWork.Country.GetAsync(c => c.CountryId == id);
      if (country == null)
      {
        throw new NotFoundException($"Country with ID: {id} not found.");
      }

      var query = MovieFeatures.Build(queryParameters);
      query.Filters.Add(c => c.CountryId == id);

      if (!_util.IsInRoles(new string[] { "ADMIN" }))
      {
        query.Filters.Add(c => c.Status == true);
      }

      country.Movies = await _unitOfWork.Movie.GetAllAsync(query);

      _response.TotalItems = country.Movies.Count();
      _response.Result = _mapper.Map<CountryMovieDto>(country);

      return Ok(_response);
    }

    [HttpGet]
    [Route("{slug}/movies")]
    public async Task<ActionResult<ResponseDto>> GetWithMovies(string slug, [FromQuery] MovieQueryParameters? queryParameters)
    {
      var country = await _unitOfWork.Country.GetAsync(c => c.Slug == slug);
      if (country == null)
      {
        throw new NotFoundException($"Country with Slug: {slug} not found.");
      }

      var query = MovieFeatures.Build(queryParameters);
      query.Filters.Add(c => c.Slug == slug);

      if (!_util.IsInRoles(new string[] { "ADMIN" }))
      {
        query.Filters.Add(c => c.Status == true);
      }

      country.Movies = await _unitOfWork.Movie.GetAllAsync(query);

      _response.TotalItems = country.Movies.Count();
      _response.Result = _mapper.Map<CountryMovieDto>(country);

      return Ok(_response);
    }

    [HttpPost]
    [Authorize(Roles ="ADMIN")]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] CountryDto countryDto)
    {

      Country country = _mapper.Map<Country>(countryDto);
      // Generate slug
      country.Slug = SlugGenerator.GenerateSlug(country.Name);

      try
      {
        await _unitOfWork.Country.AddAsync(country);
        await _unitOfWork.SaveAsync();

      }
      catch (DbUpdateException ex)
      {
        if (_util.IsUniqueConstraintViolation(ex))
        {
          country.Slug = SlugGenerator.CreateUniqueSlugAsync(country.Name);
          await _unitOfWork.Country.AddAsync(country);
          await _unitOfWork.SaveAsync();
        }
      }

      _response.Result = _mapper.Map<CountryDto>(country);

      return Created(string.Empty, _response);
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] CountryDto countryDto)
    {
      Country country = _mapper.Map<Country>(countryDto);

      Country cateFromDb = await _unitOfWork.Country.GetAsync(c => c.CountryId == countryDto.CountryId);
      // Generate slug
      if (cateFromDb.Name != country.Name)
      {
        country.Slug = SlugGenerator.GenerateSlug(country.Name);
      }

      try
      {
        await _unitOfWork.Country.UpdateAsync(country);
        await _unitOfWork.SaveAsync();

      }
      catch (DbUpdateException ex)
      {
        if (_util.IsUniqueConstraintViolation(ex))
        {
          country.Slug = SlugGenerator.CreateUniqueSlugAsync(country.Name);
          await _unitOfWork.Country.UpdateAsync(country);
          await _unitOfWork.SaveAsync();
        }
      }


      _response.Result = _mapper.Map<CountryDto>(country);

      return Ok(_response);
    }

    [HttpDelete]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var country = await _unitOfWork.Country.GetAsync(c => c.CountryId == id);
      if (country == null)
      {
        throw new NotFoundException($"Country with ID: {id} not found.");
      }

      await _unitOfWork.Country.RemoveAsync(country);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }

  }
}
