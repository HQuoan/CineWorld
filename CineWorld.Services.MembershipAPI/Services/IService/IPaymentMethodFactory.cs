namespace CineWorld.Services.MembershipAPI.Services.IService
{
  public interface IPaymentMethodFactory
  {
    IPaymentMethod GetPaymentMethod(string paymentMethod);
  }
}
