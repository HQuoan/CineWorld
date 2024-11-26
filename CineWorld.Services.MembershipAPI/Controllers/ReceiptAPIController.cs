using AutoMapper;
using CineWorld.EmailService;
using CineWorld.Services.MembershipAPI.APIFeatures;
using CineWorld.Services.MembershipAPI.Exceptions;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Models.Dto;
using CineWorld.Services.MembershipAPI.Models.Dtos;
using CineWorld.Services.MembershipAPI.Repositories;
using CineWorld.Services.MembershipAPI.Repositories.IRepositories;
using CineWorld.Services.MembershipAPI.Services.IService;
using CineWorld.Services.MembershipAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace CineWorld.Services.MembershipAPI.Controllers
{
  [Route("api/receipts")]
  [ApiController]
  [Authorize]
  public class ReceiptAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUserService _userService;

    public ReceiptAPIController(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IUserService userService)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _emailService = emailService;
      _userService = userService;
    }

    /// <summary>
    /// Retrieves all receipts with pagination support.
    /// </summary>
    /// <param name="queryParameters">Query parameters for pagination and filtering.</param>
    /// <returns>A list of receipts with pagination information.</returns>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] ReceiptQueryParameters queryParameters)
    {
      var query = ReceiptFeatures.Build(queryParameters);
      IEnumerable<Receipt> receipts = await _unitOfWork.Receipt.GetAllAsync(query);
      _response.Result = _mapper.Map<IEnumerable<ReceiptDto>>(receipts);

      int totalItems = await _unitOfWork.Receipt.CountAsync();
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
    /// Retrieves a specific receipt by its ID.
    /// </summary>
    /// <param name="id">The ID of the receipt to retrieve.</param>
    /// <returns>The details of the specified receipt.</returns>
    /// <response code="404">If the receipt is not found.</response>
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

    /// <summary>
    /// Retrieves all receipts for a specific user by their user ID.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve receipts for.</param>
    /// <returns>A list of receipts for the specified user.</returns>
    /// <response code="404">If no receipts are found for the user.</response>
    [HttpGet("{userId}")]
    public async Task<ActionResult<ResponseDto>> Get(string userId)
    {
      var query = new QueryParameters<Receipt>();
      query.Filters.Add(c => c.UserId == userId);

      IEnumerable<Receipt> receipts = await _unitOfWork.Receipt.GetAllAsync(query);

      _response.Result = _mapper.Map<IEnumerable<ReceiptDto>>(receipts);

      return Ok(_response);
    }

    /// <summary>
    /// Retrieves the receipts associated with the currently authenticated user.
    /// </summary>
    /// <returns>A list of receipts for the current user.</returns>
    /// <response code="404">If no receipts are found for the user.</response>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ResponseDto>> GetOrdersByMe()
    {
      string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

      if (userId == null)
      {
        throw new NotFoundException($"User with ID: {userId} not found.");
      }

      var query = new QueryParameters<Receipt>();
      query.Filters.Add(c => c.UserId == userId);

      IEnumerable<Receipt> receipts = await _unitOfWork.Receipt.GetAllAsync(query);

      _response.Result = _mapper.Map<IEnumerable<ReceiptDto>>(receipts);

      return Ok(_response);
    }

    /// <summary>
    /// Creates a new receipt for a user, including package details and potential discounts.
    /// </summary>
    /// <param name="receiptDto">The details of the receipt to create.</param>
    /// <returns>The created receipt with its details.</returns>
    /// <response code="400">If the user does not exist or is not authorized to create a receipt.</response>
    /// <response code="404">If the package or coupon does not exist.</response>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ResponseDto>> Post([FromBody] ReceiptDto receiptDto)
    {
      var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
      if (receiptDto.UserId == null)
      {
        receiptDto.UserId = userIdClaim.Value;
      }

      bool isExistUser = await _userService.IsExistUser(receiptDto.UserId);
      if (!isExistUser)
      {
        throw new NotFoundException($"User with ID: {receiptDto.UserId} not found.");
      }

      if (!User.IsInRole(SD.AdminRole))
      {
        if (userIdClaim != null && userIdClaim.Value == receiptDto.UserId)
        {
          receiptDto.UserId = userIdClaim.Value;
        }
        else if (userIdClaim == null) 
        {
          return BadRequest(new { Message = "UserId not found in claims." });
        }
        else
        {
          return BadRequest(new { Message = "You're not allowed to create a receipt for someone else." });
        }
      }


      Package package = await _unitOfWork.Package.GetAsync(c => c.PackageId == receiptDto.PackageId);
      if (package == null)
      {
        throw new NotFoundException($"Package with ID: {receiptDto.PackageId} not found.");
      }

      Coupon coupon = null;
      if (!string.IsNullOrEmpty(receiptDto.CouponCode))
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
      receipt.TermInMonths = package.TermInMonths;
      receipt.Status = SD.Status_Pending;
      receipt.DiscountAmount = coupon != null ? coupon.DiscountAmount : 0;
      receipt.Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;

      await _unitOfWork.Receipt.AddAsync(receipt);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<ReceiptDto>(receipt);

      return Created(string.Empty, _response);
    }

    /// <summary>
    /// Creates a Stripe checkout session for a specific receipt.
    /// </summary>
    /// <param name="stripeRequestDto">The Stripe session request details.</param>
    /// <returns>The Stripe session URL for the user to complete the payment.</returns>
    /// <response code="404">If the receipt or associated package is not found.</response>
    [HttpPost("CreateStripeSession")]
    public async Task<ActionResult<ResponseDto>> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
    {
      Receipt receiptFromDb = await _unitOfWork.Receipt.GetAsync(c => c.ReceiptId == stripeRequestDto.ReceiptId, tracked: true);

      if (receiptFromDb == null)
      {
        throw new NotFoundException($"Receipt with ID: {stripeRequestDto.ReceiptId} not found.");
      }

      if (receiptFromDb.Status == SD.Status_Approved)
      {
        _response.IsSuccess = true;
        _response.Message = "The membership subscription invoice has been paid.";

        return Ok(_response);
      }


      var options = new SessionCreateOptions
      {
        SuccessUrl = stripeRequestDto.ApprovedUrl,
        CancelUrl = stripeRequestDto.CancelUrl,
        LineItems = new List<SessionLineItemOptions>(),
        Mode = "payment",
        CustomerEmail = receiptFromDb.Email
      };
   
      Package package = await _unitOfWork.Package.GetAsync(c => c.PackageId == receiptFromDb.PackageId);
      if (package == null)
      {
        throw new NotFoundException($"Package with ID: {receiptFromDb.PackageId} not found.");
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

      if (!string.IsNullOrEmpty(receiptFromDb.CouponCode))
      {
        var discountsObj = new List<SessionDiscountOptions>
        {
            new SessionDiscountOptions
            {
                Coupon = receiptFromDb.CouponCode
            }
        };
        options.Discounts = discountsObj;
      }

      var service = new SessionService();
      Session session = await service.CreateAsync(options);
      stripeRequestDto.StripeSessionUrl = session.Url;
      receiptFromDb.StripeSessionId = session.Id;

      // Cap nhat 
      receiptFromDb.StripeSessionId = session.Id;

      await _unitOfWork.SaveAsync();
      _response.Result = stripeRequestDto;

      return Ok(_response);
    }

    /// <summary>
    /// Validates the payment status of a Stripe session and updates the receipt accordingly.
    /// </summary>
    /// <param name="receiptId">The ID of the receipt to validate.</param>
    /// <returns>The updated receipt and membership details, if payment is successful.</returns>
    /// <response code="404">If the receipt is not found.</response>
    /// <response code="400">If the payment has not been completed successfully.</response>
    [HttpPost("ValidateStripeSession/{receiptId:int}")]
    public async Task<ActionResult<ResponseDto>> ValidateStripeSession(int receiptId)
    {
      Receipt receipt = await _unitOfWork.Receipt.GetAsync(c => c.ReceiptId == receiptId);
      if (receipt == null)
      {
        throw new NotFoundException($"Receipt with ID: {receiptId} not found.");
      }

      if (receipt.Status == SD.Status_Approved)
      {
        _response.IsSuccess = true;
        _response.Message = "The membership subscription invoice has been paid.";

        return Ok(_response);
      }
      else if(receipt.Status == SD.Status_Pending)
      {
        var service = new SessionService();
        Session session = service.Get(receipt.StripeSessionId);

        var paymentIntentService = new Stripe.PaymentIntentService();

        Stripe.PaymentIntent paymentIntent;
        try
        {
          paymentIntent = paymentIntentService.Get(session.PaymentIntentId);
        }
        catch (Exception)
        {
          return BadRequest(new { Message = "You have not completed the invoice payment." });
        }

        if (paymentIntent.Status == "succeeded")
        {
          // Payment successful
          receipt.PaymentIntentId = paymentIntent.Id;
          receipt.Status = SD.Status_Approved;

          await _unitOfWork.Receipt.UpdateAsync(receipt);
          await _unitOfWork.SaveAsync();

          // Create or update Membership
          MemberShip membershipFromDb = await _unitOfWork.MemberShip.GetAsync(c => c.UserId == receipt.UserId);
          MemberShip membershipToReturn;

          if (membershipFromDb == null)
          {
            // Create new membership
            var newMemberShip = new MemberShip
            {
              UserId = receipt.UserId,
              UserEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
              FirstSubscriptionDate = DateTime.UtcNow,
              RenewalStartDate = DateTime.UtcNow,
              LastUpdatedDate = DateTime.UtcNow,
              ExpirationDate = DateTime.UtcNow.AddMonths(receipt.TermInMonths),
            };

            await _unitOfWork.MemberShip.AddAsync(newMemberShip);
            membershipToReturn = newMemberShip; // Assign for return
          }
          else
          {
            // Update existing membership
            if (membershipFromDb.ExpirationDate > DateTime.UtcNow)
            {
              // Membership still active
              membershipFromDb.ExpirationDate = membershipFromDb.ExpirationDate.AddMonths(receipt.TermInMonths);
            }
            else
            {
              // Membership expired
              membershipFromDb.ExpirationDate = DateTime.UtcNow.AddMonths(receipt.TermInMonths);
              membershipFromDb.RenewalStartDate = DateTime.UtcNow;
            }
            membershipFromDb.LastUpdatedDate = DateTime.UtcNow;

            await _unitOfWork.MemberShip.UpdateAsync(membershipFromDb);
            membershipToReturn = membershipFromDb; // Assign for return
          }

          await _unitOfWork.SaveAsync();

          // Tạo một object ẩn danh chứa cả receipt và membership
          var result = new
          {
            Receipt = _mapper.Map<ReceiptDto>(receipt),
            Membership = _mapper.Map<MemberShipDto>(membershipToReturn)
          };

          // Gửi email gia hạn gói thành công 
          var responeSendMail = await _emailService.SendEmailAsync(new EmailRequest
          {
            //To = membershipToReturn.UserEmail,
            To = receipt.Email,
            Subject = "Payment Successful",
            Message = "Payment Successful"
          });

          _response.Result = new
          {
            result,
            responeSendMail
          };
        }
        else
        {
          // Nếu thanh toán không thành công
          _response.IsSuccess = false;
          _response.Message = "Payment failed. Please try again.";
        }
      }

      return Ok(_response);
    }


    /// <summary>
    /// Deletes a receipt. Only authorized users can delete their own pending receipts, or admins can delete any receipt.
    /// </summary>
    /// <param name="id">The ID of the receipt to delete.</param>
    /// <returns>No content if deletion is successful.</returns>
    /// <response code="404">If the receipt is not found.</response>
    [HttpDelete]
    [Authorize]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      Receipt receipt = await _unitOfWork.Receipt.GetAsync(c => c.ReceiptId == id);
      if (receipt == null)
      {
        throw new NotFoundException($"Receipt with ID: {id} not found.");
      }

      string? userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
      if (User.IsInRole("ADMIN") || (userId == receipt.UserId && receipt.Status == SD.Status_Pending))
      {
        await _unitOfWork.Receipt.RemoveAsync(receipt);
        await _unitOfWork.SaveAsync();
      }

      return NoContent();
    }

  }
}
