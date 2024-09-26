using AutoMapper;
using CineWorld.Services.CouponAPI.Attributes;
using CineWorld.Services.MovieAPI.Exceptions;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CineWorld.Services.MovieAPI.Controllers
{
  [Route("api/categories")]
  [ApiController]
 // [ExceptionHandling]
  //[Authorize]
  public class CategoryAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;


    public CategoryAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
      IEnumerable<Category> categories = await _unitOfWork.Category.GetAllAsync();
      _response.Result = _mapper.Map<IEnumerable<CategoryDto>>(categories);

      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var category = await _unitOfWork.Category.GetAsync(c => c.CategoryId == id);
      if (category == null)
      {
        throw new NotFoundException($"Category with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<CategoryDto>(category);
      return Ok(_response);
    }

    [HttpGet]
    [Route("{id:int}/movies")]
    public async Task<ActionResult<ResponseDto>> GetWithMovies(int id)
    {


      var category = await _unitOfWork.Category.GetAsync(c => c.CategoryId == id, includeProperties: "Movies");
      if (category == null)
      {
        throw new NotFoundException($"Category with ID: {id} not found.");
      }

      // Remove movie with status = false
      _util.FilterMoviesByUserRole(category);

      _response.Result = _mapper.Map<CategoryMovieDto>(category);
      return Ok(_response);
    }



    [HttpPost]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] CategoryDto categoryDto)
    {
      Category category = _mapper.Map<Category>(categoryDto);
      await _unitOfWork.Category.AddAsync(category);
      await _unitOfWork.SaveAsync();
      _response.Result = _mapper.Map<CategoryDto>(category);

      return Created(string.Empty, _response);
    }

    [HttpPut]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] CategoryDto categoryDto)
    {
      Category category = _mapper.Map<Category>(categoryDto);
      await _unitOfWork.Category.UpdateAsync(category);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<CategoryDto>(category);

      return Ok(_response);
    }

    [HttpDelete]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var category = await _unitOfWork.Category.GetAsync(c => c.CategoryId == id);
      if (category == null)
      {
        throw new NotFoundException($"Category with ID: {id} not found.");
      }

      await _unitOfWork.Category.RemoveAsync(category);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }

  }
}
