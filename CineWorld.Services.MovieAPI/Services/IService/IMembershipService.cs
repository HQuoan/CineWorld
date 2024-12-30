using CineWorld.Services.MovieAPI.Models.Dtos;

namespace CineWorld.Services.MovieAPI.Services.IService
{
    public interface IMembershipService
    {
        Task<MemberShipDto> GetMembership(string membershipId);
    }
}