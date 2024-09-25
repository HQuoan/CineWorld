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
  [Route("api/categories")]
  [ApiController]
  [ExceptionHandling]
  public class CategoryAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;

    public CategoryAPIController(IUnitOfWork unitOfWork, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
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
      if(category == null)
      {
        throw new NotFoundException($"Category with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<CategoryDto>(category);
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
      if(category == null)
      {
        throw new NotFoundException($"Category with ID: {id} not found.");
      }

      await _unitOfWork.Category.RemoveAsync(category);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }

  }
}
