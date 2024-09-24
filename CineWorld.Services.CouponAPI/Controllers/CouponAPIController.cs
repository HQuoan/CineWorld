using AutoMapper;
using CineWorld.Services.CouponAPI.Attributes;
using CineWorld.Services.CouponAPI.Data;
using CineWorld.Services.CouponAPI.Exceptions;
using CineWorld.Services.CouponAPI.Models;
using CineWorld.Services.CouponAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.CouponAPI.Controllers
{
  [Route("api/coupons")]
  [ApiController]
  //[Authorize]
  [ExceptionHandling]

  public class CouponAPIController : ControllerBase
  {
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private ResponseDto _response;

    public CouponAPIController(AppDbContext db, IMapper mapper)
    {
      _db = db;
      _mapper = mapper;
      _response = new ResponseDto();
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
      IEnumerable<Coupon> coupons = await _db.Coupons.ToListAsync();
      _response.Result = _mapper.Map<IEnumerable<CouponDto>>(coupons);

      return Ok(_response);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      Coupon? coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.CouponId == id);
      if (coupon == null)
      {
        throw new NotFoundException($"Coupon with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<CouponDto>(coupon);

      return Ok(_response);
    }

    [HttpGet]
    [Route("GetByCode/{code}")]
    public async Task<ActionResult<ResponseDto>> GetByCode(string code)
    {
      Coupon? coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.CouponCode.ToLower() == code.ToLower());
      if (coupon == null)
      {
        throw new NotFoundException($"Coupon with Code: {code} not found.");
      }

      _response.Result = _mapper.Map<CouponDto>(coupon);

      return Ok(_response);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] CouponDto couponDto)
    {
      Coupon coupon = _mapper.Map<Coupon>(couponDto);
      await _db.Coupons.AddAsync(coupon); 
      await _db.SaveChangesAsync();

      _response.Result = _mapper.Map<CouponDto>(coupon);

      return Created(string.Empty, _response);
    }

    [HttpPut]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] CouponDto couponDto)
    {
      Coupon coupon = _mapper.Map<Coupon>(couponDto);
      _db.Coupons.Update(coupon);
      await _db.SaveChangesAsync();

      _response.Result = _mapper.Map<CouponDto>(coupon);

      return Ok(_response);
    }

    [HttpDelete]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.CouponId == id);
      if (coupon == null)
      {
        throw new NotFoundException($"Coupon with ID: {id} not found.");
      }

      _db.Coupons.Remove(coupon);

      await _db.SaveChangesAsync();

      return NoContent();
    }
  }
}
