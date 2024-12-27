using AutoMapper;
using CineWorld.Services.MembershipAPI.APIFeatures;
using CineWorld.Services.MembershipAPI.Exceptions;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Models.Dtos;
using CineWorld.Services.MembershipAPI.Repositories;
using CineWorld.Services.MembershipAPI.Repositories.IRepositories;
using CineWorld.Services.MembershipAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineWorld.Services.MembershipAPI.Controllers
{
  /// <summary>
  /// Handles API requests related to memberships.
  /// </summary>
  [Route("api/memberships")]
  [ApiController]
  public class MemberShipAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberShipAPIController"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="util">The utility service.</param>
    public MemberShipAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
    }

    [HttpGet]
    [Route("MemberStat")]
    public async Task<ActionResult<ResponseDto>> MemberStat()
    {
      QueryParameters<MemberShip> query = new QueryParameters<MemberShip>();
      query.Filters.Add(m => m.ExpirationDate >= DateTime.UtcNow);
      IEnumerable<MemberShip> memberShips = await _unitOfWork.MemberShip.GetAllAsync(query);

      var memberStat = memberShips
        .GroupBy(c => c.MemberType)
        .Select(s => new MemberStatResultDto
          {
            MemberType = s.Key,
            Count = s.Count()
          });

      _response.Result = memberStat;

      return Ok(_response);
    }

    /// <summary>
    /// Gets all memberships with pagination and filtering.
    /// </summary>
    /// <param name="queryParameters">The query parameters for pagination and filtering.</param>
    /// <returns>A list of memberships along with pagination information.</returns>
    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] MemberShipQueryParameters queryParameters)
    {
      var query = MemberShipFeatures.Build(queryParameters);
      IEnumerable<MemberShip> memberShips = await _unitOfWork.MemberShip.GetAllAsync(query);
      _response.Result = _mapper.Map<IEnumerable<MemberShipDto>>(memberShips);

      int totalItems = await _unitOfWork.MemberShip.CountAsync(query);
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
    /// Gets a membership by its ID.
    /// </summary>
    /// <param name="id">The ID of the membership.</param>
    /// <returns>The membership details.</returns>
    /// <exception cref="NotFoundException">Thrown if no membership is found with the given ID.</exception>
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var memberShip = await _unitOfWork.MemberShip.GetAsync(c => c.MemberShipId == id);
      if (memberShip == null)
      {
        throw new NotFoundException($"MemberShip with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<MemberShipDto>(memberShip);
      return Ok(_response);
    }

    /// <summary>
    /// Gets a membership by user ID.
    /// </summary>
    /// <param name="userId">The user ID associated with the membership.</param>
    /// <returns>The membership details.</returns>
    [HttpGet]
    [Route("GetByUserId/{userId}")]
    public async Task<ActionResult<ResponseDto>> GetByUserId(string userId)
    {
      var memberShip = await _unitOfWork.MemberShip.GetAsync(c => c.UserId == userId);
      if (memberShip == null)
      {
        return null;
      }

      _response.Result = _mapper.Map<MemberShipDto>(memberShip);
      return Ok(_response);
    }

    /// <summary>
    /// Gets a membership by user email.
    /// </summary>
    /// <param name="userEmail">The user email associated with the membership.</param>
    /// <returns>The membership details.</returns>
    /// <exception cref="NotFoundException">Thrown if no membership is found with the given email.</exception>
    [HttpGet]
    [Route("{userEmail}")]
    public async Task<ActionResult<ResponseDto>> GetByUserEmail(string userEmail)
    {
      var memberShip = await _unitOfWork.MemberShip.GetAsync(c => c.UserEmail == userEmail);
      if (memberShip == null)
      {
        throw new NotFoundException($"MemberShip with UserEmail: {userEmail} not found.");
      }

      _response.Result = _mapper.Map<MemberShipDto>(memberShip);
      return Ok(_response);
    }

    /// <summary>
    /// Creates a new membership.
    /// </summary>
    /// <param name="memberShipDto">The membership data transfer object.</param>
    /// <returns>The created membership details.</returns>
    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] MemberShipDto memberShipDto)
    {
      MemberShip memberShip = _mapper.Map<MemberShip>(memberShipDto);

      await _unitOfWork.MemberShip.AddAsync(memberShip);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<MemberShipDto>(memberShip);

      return Created(string.Empty, _response);
    }

    /// <summary>
    /// Updates an existing membership.
    /// </summary>
    /// <param name="memberShipDto">The membership data transfer object with updated values.</param>
    /// <returns>The updated membership details.</returns>
    /// <exception cref="NotFoundException">Thrown if the membership to update does not exist.</exception>
    [HttpPut]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] MemberShipDto memberShipDto)
    {
      MemberShip memberShip = _mapper.Map<MemberShip>(memberShipDto);

      MemberShip memberShipFromDb = await _unitOfWork.MemberShip.GetAsync(c => c.MemberShipId == memberShipDto.MemberShipId);
      if (memberShipFromDb == null)
      {
        throw new NotFoundException($"MemberShip with ID: {memberShipDto.MemberShipId} not found.");
      }

      memberShip.FirstSubscriptionDate = memberShipDto.FirstSubscriptionDate;

      await _unitOfWork.MemberShip.UpdateAsync(memberShip);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<MemberShipDto>(memberShip);

      return Ok(_response);
    }

    /// <summary>
    /// Deletes a membership by its ID.
    /// </summary>
    /// <param name="id">The ID of the membership to delete.</param>
    /// <returns>A no content response if successful.</returns>
    /// <exception cref="NotFoundException">Thrown if no membership is found with the given ID.</exception>
    [HttpDelete]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var memberShip = await _unitOfWork.MemberShip.GetAsync(c => c.MemberShipId == id);
      if (memberShip == null)
      {
        throw new NotFoundException($"MemberShip with ID: {id} not found.");
      }

      await _unitOfWork.MemberShip.RemoveAsync(memberShip);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }
  }

}
