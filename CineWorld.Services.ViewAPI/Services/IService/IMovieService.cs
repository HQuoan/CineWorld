using CineWorld.Services.ViewAPI.Models.Dtos;

namespace CineWorld.Services.ViewAPI.Services.IService
{
    public interface IMovieService
    {
    Task<List<GetEpsiodeWithMovieInformationDto>> GetEpsiodeWithMovieInformatio(GetEpsiodeWithMovieInformationRequestDto model);
    }
}