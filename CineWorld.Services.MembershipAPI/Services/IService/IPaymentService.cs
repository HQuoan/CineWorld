using CineWorld.Services.MembershipAPI.Models.Dtos;

namespace CineWorld.Services.MembershipAPI.Services.IService
{
    public interface IPaymentService
    {
        Task<string> CreateSession(PaymentRequestDto paymentRequestDto);
        Task<ResponseDto> ValidateSession(int receiptId);
    }
}