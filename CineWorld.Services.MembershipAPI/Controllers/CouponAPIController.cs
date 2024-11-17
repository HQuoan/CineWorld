using AutoMapper;
using CineWorld.Services.MembershipAPI.Exceptions;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Models.Dtos;
using CineWorld.Services.MembershipAPI.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace CineWorld.Services.MembershipAPI.Controllers
{
  [Route("api/coupons")]
  [ApiController]
  public class CouponAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;

    public CouponAPIController(IUnitOfWork unitOfWork, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
    }
    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
      IEnumerable<Coupon> coupons = await _unitOfWork.Coupon.GetAllAsync();
      _response.TotalItems = coupons.Count();
      _response.Result = _mapper.Map<IEnumerable<CouponDto>>(coupons);

      return Ok(_response);
    }
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

    

    [HttpPost]
    //[Authorize(Roles = SD.AdminRole)]
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

    [HttpPut]
    //[Authorize(Roles = SD.AdminRole)]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] CouponDto couponDto)
    {
      Coupon coupon = _mapper.Map<Coupon>(couponDto);

      await _unitOfWork.Coupon.UpdateAsync(coupon);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<CouponDto>(coupon);

      return Ok(_response);
    }

    [HttpDelete]
  //  [Authorize(Roles = SD.AdminRole)]
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
