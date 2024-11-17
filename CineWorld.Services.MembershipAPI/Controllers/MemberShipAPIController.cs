using AutoMapper;
using CineWorld.Services.MembershipAPI.APIFeatures;
using CineWorld.Services.MembershipAPI.Exceptions;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Models.Dtos;
using CineWorld.Services.MembershipAPI.Repositories.IRepositories;
using CineWorld.Services.MembershipAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineWorld.Services.MembershipAPI.Controllers
{
  [Route("api/memberships")]
  [ApiController]
  public class MemberShipAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;

    public MemberShipAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] MemberShipQueryParameters queryParameters)
    {
      var query = MemberShipFeatures.Build(queryParameters);
      IEnumerable<MemberShip> memberShips = await _unitOfWork.MemberShip.GetAllAsync(query);
      _response.Result = _mapper.Map<IEnumerable<MemberShipDto>>(memberShips);

      int totalItems = await _unitOfWork.MemberShip.CountAsync();
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
      var memberShip = await _unitOfWork.MemberShip.GetAsync(c => c.MemberShipId == id);
      if (memberShip == null)
      {
        throw new NotFoundException($"MemberShip with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<MemberShipDto>(memberShip);
      return Ok(_response);
    }

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

    [HttpPut]
    // [Authorize(Roles = SD.AdminRole)]
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

    [HttpDelete]
    // [Authorize(Roles = SD.AdminRole)]
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
