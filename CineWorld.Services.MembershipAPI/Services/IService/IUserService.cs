namespace CineWorld.Services.MembershipAPI.Services.IService
{
    public interface IUserService
    {
        Task<bool> IsExistUser(string userId);
    }
}