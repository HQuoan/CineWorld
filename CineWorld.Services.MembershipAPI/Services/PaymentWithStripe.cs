using CineWorld.Services.MembershipAPI.Models.Dtos;
using CineWorld.Services.MembershipAPI.Models;
using Stripe.Checkout;
using CineWorld.Services.MembershipAPI.Services.IService;

namespace CineWorld.Services.MembershipAPI.Services
{
  public class PaymentWithStripe : IPaymentMethod
  {
    public async Task<PaymentSession> CreateSession(PaymentRequestDto paymentRequestDto, Receipt receipt, Package package)
    {
      var options = new SessionCreateOptions
      {
        SuccessUrl = paymentRequestDto.ApprovedUrl,
        CancelUrl = paymentRequestDto.CancelUrl,
        LineItems = new List<SessionLineItemOptions>(),
        Mode = "payment",
        CustomerEmail = receipt.Email
      };


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

      if (!string.IsNullOrEmpty(receipt.CouponCode))
      {
        var discountsObj = new List<SessionDiscountOptions>
        {
            new SessionDiscountOptions
            {
                Coupon = receipt.CouponCode
            }
        };
        options.Discounts = discountsObj;
      }

      var service = new SessionService();
      var session = await service.CreateAsync(options);

      return new PaymentSession
      {
        Id = session.Id,
        Url = session.Url,
      };
    }


    public async Task<bool> ValidateSession(string sessionId)
    {
      var service = new SessionService();
      Session session = service.Get(sessionId);

      var paymentIntentService = new Stripe.PaymentIntentService();

      Stripe.PaymentIntent paymentIntent;
      try
      {
        paymentIntent = paymentIntentService.Get(session.PaymentIntentId);
      }
      catch (Exception)
      {
        throw new Exception("You have not completed the invoice payment.");
      }


      return paymentIntent.Status == "succeeded";
    }
  }
}
