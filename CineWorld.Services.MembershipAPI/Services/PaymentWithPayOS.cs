using CineWorld.Services.MembershipAPI.Models.Dtos;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Services.IService;
using CineWorld.Services.MembershipAPI.Utilities;
using Net.payOS;
using Net.payOS.Types;
using Microsoft.Extensions.Options;

namespace CineWorld.Services.MembershipAPI.Services
{
  public class PaymentWithPayOS : IPaymentMethod
  {
    private readonly PayOSOptions _payOSOptions;
    public PaymentWithPayOS(IOptions<PayOSOptions> payOSOptions)
    {
      _payOSOptions = payOSOptions.Value;
    }
    public string PaymentMethodName => SD.PaymentWithPayOS;
    public async Task<PaymentSession> CreateSession(PaymentRequestDto paymentRequestDto, Receipt receipt, Package package)
    {
      PayOS payOS = new PayOS(_payOSOptions.ClientId, _payOSOptions.ApiKey, _payOSOptions.ChecksumKey);

      var item = new ItemData(
          package.Name,
          1,
          1000
      );

      string currentDate = DateTime.Now.ToString("ddMMyy");
      long orderCode = long.Parse($"{receipt.ReceiptId.ToString()}{currentDate}");

      PaymentData paymentData = new PaymentData(
          orderCode,
          1000,
          package.Name,
          new List<ItemData> { item },
          paymentRequestDto.CancelUrl,
          paymentRequestDto.ApprovedUrl
      );

      CreatePaymentResult createPayment = await payOS.createPaymentLink(paymentData);

      if (createPayment == null || string.IsNullOrEmpty(createPayment.paymentLinkId))
      {
          throw new InvalidOperationException("Unable to create payment link. Please try again later.");
      }

      return new PaymentSession
      {
        Id = orderCode.ToString(),
        Url = createPayment.checkoutUrl,
      };
    }


    public async Task<bool> ValidateSession(string sessionId)
    {
      var payment = await GetPaymentInformation(sessionId);

      return payment.status == "PAID";
    }

    public async Task<PaymentLinkInformation> GetPaymentInformation(string sessionId)
    {
     PayOS payOS = new PayOS(_payOSOptions.ClientId, _payOSOptions.ApiKey, _payOSOptions.ChecksumKey);

      return await payOS.getPaymentLinkInformation(long.Parse(sessionId));
    }
  }
}
