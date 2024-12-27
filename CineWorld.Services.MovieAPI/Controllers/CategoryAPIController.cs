using AutoMapper;
using CineWorld.Services.MovieAPI.APIFeatures;
using CineWorld.Services.MovieAPI.Exceptions;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.MovieAPI.Controllers
{
  /// <summary>
  /// Controller responsible for handling Category-related API requests.
  /// Provides functionality for managing categories and their associated movies.
  /// </summary>
  [Route("api/categories")]
  [ApiController]
  public class CategoryAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;

    /// <summary>
    /// Initializes a new instance of the CategoryAPIController class.
    /// </summary>
    public CategoryAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
    }

    /// <summary>
    /// Retrieves a list of categories with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="queryParameters">Optional query parameters for filtering and pagination.</param>
    /// <returns>A response containing the list of categories with pagination data.</returns>
    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] CategoryQueryParameters queryParameters)
    {
      var query = CategoryFeatures.Build(queryParameters);
      IEnumerable<Category> categories = await _unitOfWork.Category.GetAllAsync(query);

      _response.Result = _mapper.Map<IEnumerable<CategoryDto>>(categories);

      int totalItems = await _unitOfWork.Category.CountAsync(query);
      _response.Pagination = new PaginationDto
      {
        TotalItems = totalItems,
        TotalItemsPerPage = queryParameters.PageSize,
        CurrentPage = queryParameters.PageNumber,
        TotalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize)
      };

      return Ok(_response);
    }

    /// <summary>
    /// Retrieves a category by its ID.
    /// </summary>
    /// <param name="id">The ID of the category.</param>
    /// <returns>A response containing the category data.</returns>
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

    /// <summary>
    /// Retrieves a category by its slug.
    /// </summary>
    /// <param name="slug">The slug of the category.</param>
    /// <returns>A response containing the category data.</returns>
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

    /// <summary>
    /// Retrieves movies associated with a category by its ID with optional filtering.
    /// </summary>
    /// <param name="id">The ID of the category.</param>
    /// <param name="queryParameters">Optional query parameters for filtering movies.</param>
    /// <returns>A response containing the category and its associated movies.</returns>
    [HttpGet]
    [Route("{id:int}/movies")]
    public async Task<ActionResult<ResponseDto>> GetWithMovies(int id, [FromQuery] MovieQueryParameters? queryParameters)
    {
      var category = await _unitOfWork.Category.GetAsync(c => c.CategoryId == id);
      if (category == null)
      {
        throw new NotFoundException($"Category with ID: {id} not found.");
      }

      var query = MovieFeatures.Build(queryParameters);
      query.Filters.Add(c => c.CategoryId == id);

      if (!User.IsInRole(SD.AdminRole))
      {
        query.Filters.Add(c => c.Status == true);
      }

      category.Movies = await _unitOfWork.Movie.GetAllAsync();

      _response.Result = _mapper.Map<CategoryMovieDto>(category);

      return Ok(_response);
    }

    /// <summary>
    /// Retrieves movies associated with a category by its slug with optional filtering.
    /// </summary>
    /// <param name="slug">The slug of the category.</param>
    /// <param name="queryParameters">Optional query parameters for filtering movies.</param>
    /// <returns>A response containing the category and its associated movies.</returns>
    [HttpGet]
    [Route("{slug}/movies")]
    public async Task<ActionResult<ResponseDto>> GetWithMovies(string slug, [FromQuery] MovieQueryParameters? queryParameters)
    {
      var category = await _unitOfWork.Category.GetAsync(c => c.Slug == slug);
      if (category == null)
      {
        throw new NotFoundException($"Category with Slug: {slug} not found.");
      }

      var query = MovieFeatures.Build(queryParameters);
      query.Filters.Add(c => c.Slug == slug);

      if (!User.IsInRole(SD.AdminRole))
      {
        query.Filters.Add(c => c.Status == true);
      }

      category.Movies = await _unitOfWork.Movie.GetAllAsync(query);

      _response.Result = _mapper.Map<CategoryMovieDto>(category);

      return Ok(_response);
    }

    /// <summary>
    /// Creates a new category.
    /// Only accessible to users with admin role.
    /// </summary>
    /// <param name="categoryDto">The category data to be created.</param>
    /// <returns>A response containing the created category data.</returns>
    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
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

    /// <summary>
    /// Updates an existing category.
    /// Only accessible to users with admin role.
    /// </summary>
    /// <param name="categoryDto">The category data to be updated.</param>
    /// <returns>A response containing the updated category data.</returns>
    [HttpPut]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] CategoryDto categoryDto)
    {
      Category category = _mapper.Map<Category>(categoryDto);

      Category cateFromDb = await _unitOfWork.Category.GetAsync(c => c.CategoryId == categoryDto.CategoryId);
      if (cateFromDb == null)
      {
        throw new NotFoundException($"Category with ID: {categoryDto.CategoryId} not found.");
      }
      // Generate slug
      if (cateFromDb.Name != category.Name)
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

    /// <summary>
    /// Deletes a category by its ID.
    /// Only accessible to users with admin role.
    /// </summary>
    /// <param name="id">The ID of the category to be deleted.</param>
    /// <returns>A NoContent response indicating successful deletion.</returns>
    [HttpDelete]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult> Delete(int id)
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
