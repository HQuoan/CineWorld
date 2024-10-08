using AutoMapper;
using CineWorld.Services.MembershipAPI.Exceptions;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Models.Dto;
using CineWorld.Services.MembershipAPI.Models.Dtos;
using CineWorld.Services.MembershipAPI.Repositories;
using CineWorld.Services.MembershipAPI.Repositories.IRepositories;
using CineWorld.Services.MembershipAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.IdentityModel.Tokens.Jwt;

namespace CineWorld.Services.MembershipAPI.Controllers
{
  [Route("api/receipts")]
  [ApiController]
  public class ReceiptAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUtil _util;

    public ReceiptAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _util = util;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get()
    {
      IEnumerable<Receipt> receipts = await _unitOfWork.Receipt.GetAllAsync();
      _response.TotalItems = receipts.Count();
      _response.Result = _mapper.Map<IEnumerable<ReceiptDto>>(receipts);

      return Ok(_response);
    }
    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<ResponseDto>> Get(int id)
    {
      var receipt = await _unitOfWork.Receipt.GetAsync(c => c.ReceiptId == id);
      if (receipt == null)
      {
        throw new NotFoundException($"Receipt with ID: {id} not found.");
      }

      _response.Result = _mapper.Map<ReceiptDto>(receipt);
      return Ok(_response);
    }

    [HttpGet("{userId}")]
   // [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Get(string userId)
    {
      var query = new QueryParameters<Receipt>();
      query.Filters.Add(c => c.UserId == userId);

      IEnumerable<Receipt> receipts = await _unitOfWork.Receipt.GetAllAsync(query);

      _response.Result = _mapper.Map<IEnumerable<ReceiptDto>>(receipts);

      return Ok(_response);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ResponseDto>> GetOrdersByMe()
    {
      string userId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

      if(userId == null)
      {
        throw new NotFoundException($"User with ID: {userId} not found.");
      }

      var query = new QueryParameters<Receipt>();
      query.Filters.Add(c => c.UserId == userId);

      IEnumerable<Receipt> receipts = await _unitOfWork.Receipt.GetAllAsync(query);

      _response.Result = _mapper.Map<IEnumerable<ReceiptDto>>(receipts);

      return Ok(_response);
    }


    [HttpPost]
    //[Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] ReceiptDto receiptDto)
    {
      Package package = await _unitOfWork.Package.GetAsync(c => c.PackageId == receiptDto.PackageId);
      if (package == null)
      {
        throw new NotFoundException($"Package with ID: {receiptDto.PackageId} not found.");
      }

      Coupon coupon = null;
      if (string.IsNullOrEmpty(receiptDto.CouponCode))
      {
         coupon = await _unitOfWork.Coupon.GetAsync(c => c.CouponCode == receiptDto.CouponCode);
        // ở đây mới chỉ check là coupon có tồn tại hay không, chưa check ngày, số lần dùng 
        if (coupon == null)
        {
          throw new NotFoundException("Coupon is invalid");
        }
      }

      Receipt receipt = _mapper.Map<Receipt>(receiptDto);
      receipt.PackagePrice = package.Price;
      receipt.Status = SD.Status_Pending;
      receipt.DiscountAmount = coupon != null ? coupon.DiscountAmount : 0;

      await _unitOfWork.Receipt.AddAsync(receipt);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<ReceiptDto>(receipt);

      return Created(string.Empty, _response);
    }

    [HttpPost("CreateStripeSession")]
    public async Task<ActionResult<ResponseDto>> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
    {
      var options = new SessionCreateOptions
      {
        SuccessUrl = stripeRequestDto.ApprovedUrl,
        CancelUrl = stripeRequestDto.CancelUrl,
        LineItems = new List<SessionLineItemOptions>(),
        Mode = "payment",
      };

   
      Package package = await _unitOfWork.Package.GetAsync(c => c.PackageId == stripeRequestDto.Receipt.PackageId);
      if (package == null)
      {
        throw new NotFoundException($"Package with ID: {stripeRequestDto.Receipt.PackageId} not found.");
      }

      var sessionLineItem = new SessionLineItemOptions
      {
        PriceData = new SessionLineItemPriceDataOptions
        {
          UnitAmount = (long)(package.Price * 100), // $20.99 -> 2099
          Currency = "usd",
          ProductData = new SessionLineItemPriceDataProductDataOptions
          {
            Name = package.Name,
          }
        },
        Quantity = 1
      };

      options.LineItems.Add(sessionLineItem);

      if (!string.IsNullOrEmpty(stripeRequestDto.Receipt.CouponCode))
      {
        var discountsObj = new List<SessionDiscountOptions>
        {
            new SessionDiscountOptions
            {
                Coupon = stripeRequestDto.Receipt.CouponCode
            }
        };
        options.Discounts = discountsObj;
      }

      var service = new SessionService();
      Session session = await service.CreateAsync(options);
      stripeRequestDto.StripeSessionUrl = session.Url;
      stripeRequestDto.Receipt.StripeSessionId = session.Id;

      Receipt receipt = await _unitOfWork.Receipt.GetAsync(c => c.ReceiptId == stripeRequestDto.Receipt.ReceiptId, tracked: true);

      receipt.StripeSessionId = session.Id;

      await _unitOfWork.SaveAsync();
      _response.Result = stripeRequestDto;

      return Ok(_response);
    }


    [HttpPost("ValidateStripeSession")]
    public async Task<ActionResult<ResponseDto>> ValidateStripeSession([FromBody] int receiptId)
    {
      Receipt receipt = await _unitOfWork.Receipt.GetAsync(c => c.ReceiptId == receiptId, tracked: true);

      var service = new SessionService();
      Session session = service.Get(receipt.StripeSessionId);

      var paymentIntentService = new Stripe.PaymentIntentService();
      Stripe.PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

      if (paymentIntent.Status == "succeeded")
      {
        // then payment was successful
        receipt.PaymentIntentId = paymentIntent.Id;
        receipt.Status = SD.Status_Approved;
        await _unitOfWork.SaveAsync();
      }

      _response.Result = _mapper.Map<ReceiptDto>(receipt);

      return Ok(_response);
    }

    [HttpDelete]
   // [Authorize]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      Receipt receipt = await _unitOfWork.Receipt.GetAsync(c => c.ReceiptId == id);
      if (receipt == null)
      {
        throw new NotFoundException($"Receipt with ID: {id} not found.");
      }

      string? userId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
      if(User.IsInRole("ADMIN") || (userId == receipt.UserId && receipt.Status == SD.Status_Pending))
      {
        await _unitOfWork.Receipt.RemoveAsync(receipt);
        await _unitOfWork.SaveAsync();
      }

      return NoContent();
    }

  }
}
