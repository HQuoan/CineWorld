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
  /// <summary>
  /// Handles API requests related to packages.
  /// </summary>
  [Route("api/packages")]
  [ApiController]
  public class PackageAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageAPIController"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public PackageAPIController(IUnitOfWork unitOfWork, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
    }

    /// <summary>
    /// Gets all packages with pagination and filtering.
    /// </summary>
    /// <param name="queryParameters">The query parameters for pagination and filtering.</param>
    /// <returns>A list of packages along with pagination information.</returns>
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

    /// <summary>
    /// Gets a package by its ID.
    /// </summary>
    /// <param name="id">The ID of the package.</param>
    /// <returns>The package details.</returns>
    /// <exception cref="NotFoundException">Thrown if no package is found with the given ID.</exception>
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

    /// <summary>
    /// Creates a new package.
    /// </summary>
    /// <param name="packageDto">The package data transfer object.</param>
    /// <returns>The created package details.</returns>
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

    /// <summary>
    /// Updates an existing package.
    /// </summary>
    /// <param name="packageDto">The package data transfer object with updated values.</param>
    /// <returns>The updated package details.</returns>
    /// <exception cref="NotFoundException">Thrown if the package to update does not exist.</exception>
    [HttpPut]
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

    /// <summary>
    /// Deletes a package by its ID.
    /// </summary>
    /// <param name="id">The ID of the package to delete.</param>
    /// <returns>A no content response if successful.</returns>
    /// <exception cref="NotFoundException">Thrown if no package is found with the given ID.</exception>
    [HttpDelete]
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
