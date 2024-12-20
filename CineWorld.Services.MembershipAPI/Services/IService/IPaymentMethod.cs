using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Models.Dtos;

namespace CineWorld.Services.MembershipAPI.Services.IService
{
  public interface IPaymentMethod
  {
    string PaymentMethodName { get; }
    Task<PaymentSession> CreateSession(PaymentRequestDto paymentRequestDto, Receipt receipt, Package package);
    Task<bool> ValidateSession(string receiptId);
  }
}