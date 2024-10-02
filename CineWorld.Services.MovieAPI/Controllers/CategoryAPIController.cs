using AutoMapper;
using CineWorld.Services.MovieAPI.Data;
using CineWorld.Services.MovieAPI.Exceptions;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CineWorld.Services.MovieAPI.Controllers
{
  [Route("api/categories")]
  [ApiController]
  //[Authorize]
  public class CategoryAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;
    private readonly AppDbContext _db;


    public CategoryAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util, AppDbContext db)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
      _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
      IEnumerable<Category> categories = await _unitOfWork.Category.GetAllAsync(new QueryParameters<Category>());
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
    [Route("{slug}")]
    public async Task<ActionResult<ResponseDto>> Get(string slug)
    { 
      var category = await _unitOfWork.Category.GetAsync(c => c.Slug == slug);
      if (category == null)
      {
        throw new NotFoundException($"Category with Slug: {slug} not found.");
      }

      _response.Result = _mapper.Map<CategoryDto>(category);
      return Ok(_response);
    }

    [HttpGet]
    [Route("{id:int}/movies")]
    public async Task<ActionResult<ResponseDto>> GetWithMovies(int id)
    {

      var category = await _unitOfWork.Category.GetAsync(c => c.CategoryId == id);
      //category.Movies = await _unitOfWork.Movie.GetAllAsync(c => c.CategoryId == category.CategoryId);
      if (category == null)
      {
        throw new NotFoundException($"Category with ID: {id} not found.");
      }

      // Remove movie with status = false

      _util.FilterMoviesByUserRole(category);

      _response.Result = _mapper.Map<CategoryMovieDto>(category);
      return Ok(_response);
    }

    [HttpGet]
    [Route("{slug}/movies")]
    public async Task<ActionResult<ResponseDto>> GetWithMovies(string slug)
    {
      var category = await _unitOfWork.Category.GetAsync(c => c.Slug == slug);
     // category.Movies = await _unitOfWork.Movie.GetAllAsync(c => c.CategoryId == category.CategoryId);
      if (category == null)
      {
        throw new NotFoundException($"Category with Slug: {slug} not found.");
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
      // Generate slug
      category.Slug = SlugGenerator.GenerateSlug(category.Name);

      try
      {
        await _unitOfWork.Category.AddAsync(category);
        await _unitOfWork.SaveAsync();
        
      }
      catch (DbUpdateException ex) 
      {
        if (_util.IsUniqueConstraintViolation(ex))
        {
          category.Slug = SlugGenerator.CreateUniqueSlugAsync(category.Name);
          await _unitOfWork.Category.AddAsync(category);
          await _unitOfWork.SaveAsync();
        }
      }
  

      _response.Result = _mapper.Map<CategoryDto>(category);

      return Created(string.Empty, _response);
    }
   


    [HttpPut]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] CategoryDto categoryDto)
    {
      Category category = _mapper.Map<Category>(categoryDto);

      Category cateFromDb = await _unitOfWork.Category.GetAsync(c=> c.CategoryId ==  categoryDto.CategoryId);
      // Generate slug
      if(cateFromDb.Name != category.Name)
      {
        category.Slug = SlugGenerator.GenerateSlug(category.Name);
      }

      try
      {
        await _unitOfWork.Category.UpdateAsync(category);
        await _unitOfWork.SaveAsync();

      }
      catch (DbUpdateException ex)
      {
        if (_util.IsUniqueConstraintViolation(ex))
        {
          category.Slug = SlugGenerator.CreateUniqueSlugAsync(category.Name);
          await _unitOfWork.Category.UpdateAsync(category);
          await _unitOfWork.SaveAsync();
        }
      }


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
