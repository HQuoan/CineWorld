using AutoMapper;
using CineWorld.Services.MembershipAPI.APIFeatures;
using CineWorld.Services.MembershipAPI.Exceptions;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Models.Dtos;
using CineWorld.Services.MembershipAPI.Repositories.IRepositories;
using CineWorld.Services.MembershipAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CineWorld.Services.MembershipAPI.Controllers
{
  [Route("api/packages")]
  [ApiController]
  public class PackageAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;

    public PackageAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] PackageQueryParameters queryParameters)
    {
      var query = PackageFeatures.Build(queryParameters);
      IEnumerable<Package> packages = await _unitOfWork.Package.GetAllAsync(query);
      _response.Result = _mapper.Map<IEnumerable<PackageDto>>(packages);

      int totalItems = await _unitOfWork.Package.CountAsync();
      _response.Pagination = new PaginationDto
      {
        TotalItems = totalItems,
        TotalItemsPerPage = queryParameters.PageSize,
        CurrentPage = queryParameters.PageNumber,
        TotalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize)
      };

      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var package = await _unitOfWork.Package.GetAsync(c => c.PackageId == id);
      if (package == null)
      {
        throw new NotFoundException($"Package with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<PackageDto>(package);
      return Ok(_response);
    }

    [HttpPost]
    //[Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] PackageDto packageDto)
    {

      Package package = _mapper.Map<Package>(packageDto);

      await _unitOfWork.Package.AddAsync(package);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<PackageDto>(package);

      return Created(string.Empty, _response);
    }

    [HttpPut]
    // [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] PackageDto packageDto)
    {
      Package package = _mapper.Map<Package>(packageDto);

      Package packageFromDb = await _unitOfWork.Package.GetAsync(c => c.PackageId == packageDto.PackageId);
      if (packageFromDb == null)
      {
        throw new NotFoundException($"Package with ID: {packageDto.PackageId} not found.");
      }

      package.UpdatedDate = DateTime.UtcNow;

      await _unitOfWork.Package.UpdateAsync(package);
      await _unitOfWork.SaveAsync();


      _response.Result = _mapper.Map<PackageDto>(package);

      return Ok(_response);
    }

    [HttpDelete]
    // [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var package = await _unitOfWork.Package.GetAsync(c => c.PackageId == id);
      if (package == null)
      {
        throw new NotFoundException($"Package with ID: {id} not found.");
      }

      await _unitOfWork.Package.RemoveAsync(package);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }

  }
}
