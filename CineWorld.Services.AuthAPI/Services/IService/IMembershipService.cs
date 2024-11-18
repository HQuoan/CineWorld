using CineWorld.Services.AuthAPI.Models.Dto;

namespace CineWorld.Services.AuthAPI.Services.IService
{
    public interface IMembershipService
    {
        Task<MemberShipDto> GetMembership(string membershipId);
    }
}