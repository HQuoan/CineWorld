using AutoMapper;
using CineWorld.EmailService;
using CineWorld.Services.MembershipAPI.APIFeatures;
using CineWorld.Services.MembershipAPI.Exceptions;
using CineWorld.Services.MembershipAPI.Models;
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
  //[Authorize]
  public class ReceiptAPIController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private ResponseDto _response;
    private readonly IUserService _userService;
    private readonly IPaymentService _paymentService;

    public ReceiptAPIController(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IUserService userService, IPaymentService paymentService)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _response = new ResponseDto();
      _emailService = emailService;
      _userService = userService;
      _paymentService = paymentService;
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
    /// <param name="paymentRequestDto">The payment session request details.</param>
    /// <returns>The payment session URL for the user to complete the payment.</returns>
    /// <response code="404">If the receipt or associated package is not found.</response>
    [HttpPost("CreateSession")]
    public async Task<ActionResult<ResponseDto>> CreateSession([FromBody] PaymentRequestDto paymentRequestDto)
    {
      var paymentSessionUrl = await _paymentService.CreateSession(paymentRequestDto);
      paymentRequestDto.PaymentSessionUrl = paymentSessionUrl;
      _response.Result = paymentRequestDto;
      _response.Message = "Payment link created successfully.";

      return Ok(_response);
    }

    /// <summary>
    /// Validates the payment status of a Stripe session and updates the receipt accordingly.
    /// </summary>
    /// <param name="receiptId">The ID of the receipt to validate.</param>
    /// <returns>The updated receipt and membership details, if payment is successful.</returns>
    /// <response code="404">If the receipt is not found.</response>
    /// <response code="400">If the payment has not been completed successfully.</response>
    [HttpPost("ValidateSession/{receiptId:int}")]
    public async Task<ActionResult<ResponseDto>> ValidateSession(int receiptId)
    {
      return Ok(await _paymentService.ValidateSession(receiptId));
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
