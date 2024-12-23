using CineWorld.Services.ViewAPI.Models.Dtos;

namespace CineWorld.Services.ViewAPI.Services.IService
{
    public interface IMovieService
    {
    Task<List<EpisodeInforDto>> GetEpsiodesInfor(IdsRequestDto model);
    Task<List<MovieInforDto>> GetMoviesInfor(IdsRequestDto model);
    Task<string> IncreaseMovieView(IncreaseMovieViewDto model);
    }
}