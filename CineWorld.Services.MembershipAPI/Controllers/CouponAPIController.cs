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
  /// <summary>
  /// Controller for managing coupons. It provides endpoints for creating, retrieving, updating, and deleting coupons.
  /// Coupons can be retrieved by ID or code, and can be listed with pagination.
  /// Admin roles are required to create, update, or delete coupons.
  /// </summary>
  [Route("api/coupons")]
  [ApiController]
  public class CouponAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;

    /// <summary>
    /// Initializes a new instance of the CouponAPIController.
    /// </summary>
    /// <param name="unitOfWork">Unit of work for accessing repository services.</param>
    /// <param name="mapper">AutoMapper instance for mapping DTOs.</param>
    public CouponAPIController(IUnitOfWork unitOfWork, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
    }

    /// <summary>
    /// Retrieves a paginated list of coupons based on query parameters.
    /// </summary>
    /// <param name="queryParameters">Query parameters for pagination and filtering.</param>
    /// <returns>A paginated list of coupons.</returns>
    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] CouponQueryParameters queryParameters)
    {
      var query = CouponFeatures.Build(queryParameters);
      IEnumerable<Coupon> coupons = await _unitOfWork.Coupon.GetAllAsync(query);

      _response.Result = _mapper.Map<IEnumerable<CouponDto>>(coupons);

      int totalItems = await _unitOfWork.Coupon.CountAsync(query);
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
    /// Retrieves a coupon by its unique ID.
    /// </summary>
    /// <param name="id">The unique identifier of the coupon.</param>
    /// <returns>A single coupon matching the ID.</returns>
    /// <exception cref="NotFoundException">Thrown if the coupon is not found.</exception>
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var coupon = await _unitOfWork.Coupon.GetAsync(c => c.CouponId == id);
      if (coupon == null)
      {
        throw new NotFoundException($"Coupon with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<CouponDto>(coupon);
      return Ok(_response);
    }

    /// <summary>
    /// Retrieves a coupon by its unique code.
    /// </summary>
    /// <param name="code">The unique coupon code.</param>
    /// <returns>A single coupon matching the code.</returns>
    /// <exception cref="NotFoundException">Thrown if the coupon is not found.</exception>
    [HttpGet]
    [Route("{code}")]
    public async Task<ActionResult<ResponseDto>> Get(string code)
    {
      var coupon = await _unitOfWork.Coupon.GetAsync(c => c.CouponCode == code);
      if (coupon == null)
      {
        throw new NotFoundException($"Coupon with Code: {code} not found.");
      }

      _response.Result = _mapper.Map<CouponDto>(coupon);
      return Ok(_response);
    }

    /// <summary>
    /// Creates a new coupon. Only accessible by users with Admin role.
    /// </summary>
    /// <param name="couponDto">The coupon data transfer object.</param>
    /// <returns>The created coupon.</returns>
    [HttpPost]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] CouponDto couponDto)
    {
      Coupon coupon = _mapper.Map<Coupon>(couponDto);

      await _unitOfWork.Coupon.AddAsync(coupon);
      await _unitOfWork.SaveAsync();

      // create stripe coupon
      var options = new Stripe.CouponCreateOptions
      {
        Id = couponDto.CouponCode,
        AmountOff = (long)(couponDto.DiscountAmount * 100),
        Name = couponDto.CouponCode,
        Currency = "usd",
        Duration = couponDto.Duration,
        DurationInMonths = coupon.Duration == "repeating" ? (long?)couponDto.DurationInMonths : null,
        MaxRedemptions = coupon.UsageLimit > 0 ? (long?)coupon.UsageLimit : null,
      };

      var service = new Stripe.CouponService();
      await service.CreateAsync(options);

      _response.Result = _mapper.Map<CouponDto>(coupon);

      return Created(string.Empty, _response);
    }

    /// <summary>
    /// Updates an existing coupon. Only accessible by users with Admin role.
    /// </summary>
    /// <param name="couponDto">The coupon data transfer object to update.</param>
    /// <returns>The updated coupon.</returns>
    [HttpPut]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] CouponDto couponDto)
    {
      Coupon coupon = _mapper.Map<Coupon>(couponDto);

      await _unitOfWork.Coupon.UpdateAsync(coupon);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<CouponDto>(coupon);

      return Ok(_response);
    }

    /// <summary>
    /// Deletes a coupon by its unique ID. Only accessible by users with Admin role.
    /// </summary>
    /// <param name="id">The unique identifier of the coupon to delete.</param>
    /// <returns>No content if the coupon is successfully deleted.</returns>
    /// <exception cref="NotFoundException">Thrown if the coupon is not found.</exception>
    [HttpDelete]
    [Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var coupon = await _unitOfWork.Coupon.GetAsync(c => c.CouponId == id);
      if (coupon == null)
      {
        throw new NotFoundException($"Coupon with ID: {id} not found.");
      }

      await _unitOfWork.Coupon.RemoveAsync(coupon);
      await _unitOfWork.SaveAsync();

      // delete stripe coupon 
      var service = new Stripe.CouponService();
      service.Delete(coupon.CouponCode);

      return NoContent();
    }
  }
}
