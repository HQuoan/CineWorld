using AutoMapper;
using CineWorld.EmailService;
using CineWorld.Services.MembershipAPI.Exceptions;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Models.Dtos;
using CineWorld.Services.MembershipAPI.Repositories.IRepositories;
using CineWorld.Services.MembershipAPI.Services.IService;
using CineWorld.Services.MembershipAPI.Utilities;

namespace CineWorld.Services.MembershipAPI.Services
{
  public class PaymentService : IPaymentService
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly IPaymentMethodFactory _paymentMethodFactory;

    public PaymentService(IUnitOfWork unitOfWork, IEmailService emailService, IMapper mapper, IPaymentMethodFactory paymentMethodFactory)
    {
      _unitOfWork = unitOfWork;
      _emailService = emailService;
      _mapper = mapper;
      _paymentMethodFactory = paymentMethodFactory;
    }

    public async Task<string> CreateSession(PaymentRequestDto paymentRequestDto)
    {
      Receipt receiptFromDb = await _unitOfWork.Receipt.GetAsync(c => c.ReceiptId == paymentRequestDto.ReceiptId, tracked: true);

      if (receiptFromDb == null)
      {
        throw new NotFoundException($"Receipt with ID: {paymentRequestDto.ReceiptId} not found.");
      }


      if (receiptFromDb.Status == SD.Status_Session_Created)
      {
        return receiptFromDb.PaymentSessionUrl;
      }

      if (receiptFromDb.Status == SD.Status_Approved)
      {
        throw new InvalidOperationException("The membership subscription invoice has already been paid.");
      }


      Package package = await _unitOfWork.Package.GetAsync(c => c.PackageId == receiptFromDb.PackageId);
      if (package == null)
      {
        throw new NotFoundException($"Package with ID: {receiptFromDb.PackageId} not found.");
      }
      var paymentMethod = _paymentMethodFactory.GetPaymentMethod(receiptFromDb.PaymentMethod);

      var session = await paymentMethod.CreateSession(paymentRequestDto, receiptFromDb, package);

      // Cap nhat 
      receiptFromDb.StripeSessionId = session.Id;
      receiptFromDb.PaymentSessionUrl = session.Url;
      receiptFromDb.Status = SD.Status_Session_Created;

      await _unitOfWork.SaveAsync();

      return session.Url;
    }

    public async Task<ResponseDto> ValidateSession(int receiptId)
    {
      ResponseDto responseDto = new ResponseDto();
      Receipt receipt = await _unitOfWork.Receipt.GetAsync(c => c.ReceiptId == receiptId);
      if (receipt == null)
      {
        throw new NotFoundException($"Receipt with ID: {receiptId} not found.");
      }

      if (receipt.Status == SD.Status_Approved)
      {
        responseDto.IsSuccess = true;
        responseDto.Message = "The membership subscription invoice has been paid.";

        return responseDto;
      }
      else if (receipt.Status == SD.Status_Session_Created)
      {
        var paymentMethod = _paymentMethodFactory.GetPaymentMethod(receipt.PaymentMethod);
        if (await paymentMethod.ValidateSession(receipt.StripeSessionId))
        {
          // Payment successful
          //receipt.PaymentIntentId = paymentIntent.Id;
          receipt.Status = SD.Status_Approved;
          receipt.PaymentSessionUrl = null;

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
              UserEmail = receipt.Email,
              MemberType = SD.FirstTimeMember,
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
              membershipFromDb.MemberType = SD.ConsecutiveMember;
            }
            else
            {
              // Membership expired
              membershipFromDb.ExpirationDate = DateTime.UtcNow.AddMonths(receipt.TermInMonths);
              membershipFromDb.RenewalStartDate = DateTime.UtcNow;

              membershipFromDb.MemberType = SD.ReturningMember;
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
          Package package = await _unitOfWork.Package.GetAsync(c => c.PackageId == receipt.PackageId);

          var responeSendMail = await _emailService.SendEmailAsync(new EmailRequest
          {
            //To = membershipToReturn.UserEmail,
            To = receipt.Email,
            Subject = "Payment Successful",
            Message = GenerateEmailBody.PaymentSuccess(receipt, package, membershipToReturn)
          });

          responseDto.Result = new
          {
            result,
            responeSendMail
          };
        }
        else
        {
          // Nếu thanh toán không thành công
          responseDto.IsSuccess = false;
          responseDto.Message = "Payment failed. Please try again.";
        }
      }

      return responseDto;
    }

    

  }
}
