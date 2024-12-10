using CineWorld.Services.MembershipAPI.Models.Dtos;
using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Services.IService;
using CineWorld.Services.MembershipAPI.Utilities;
using Net.payOS;
using Net.payOS.Types;

namespace CineWorld.Services.MembershipAPI.Services
{
  public class PaymentWithPayOS : IPaymentMethod
  {
    public string PaymentMethodName => SD.PaymentWithPayOS;
    public async Task<PaymentSession> CreateSession(PaymentRequestDto paymentRequestDto, Receipt receipt, Package package)
    {
      PayOS payOS = new PayOS("6e63ed43-fe2c-4f2d-81c3-d63289a396c6", "32e3fe0e-c32a-4105-b9cb-36cc3a835148", "b0d0150dbbd822e0794a13139341d228ac0158d63b436456fe6210583fa635d7");

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
        throw new Exception("Unable to create payment link. Please try again later.");
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
      PayOS payOS = new PayOS("6e63ed43-fe2c-4f2d-81c3-d63289a396c6", "32e3fe0e-c32a-4105-b9cb-36cc3a835148", "b0d0150dbbd822e0794a13139341d228ac0158d63b436456fe6210583fa635d7");

      return await payOS.getPaymentLinkInformation(long.Parse(sessionId));
    }
  }
}
