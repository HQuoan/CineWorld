using AutoMapper;
using CineWorld.Services.ReactionAPI.Models.Common;
using CineWorld.Services.ReactionAPI.Models.Dtos;
using CineWorld.Services.ReactionAPI.Models.Dtos.WatchHistory;
using CineWorld.Services.ReactionAPI.Models.Entities;
using CineWorld.Services.ReactionAPI.Models.ReqParams;
using CineWorld.Services.ReactionAPI.Repositories.Interface;
using CineWorld.Services.ReactionAPI.Services.Interface;
using Newtonsoft.Json;

namespace CineWorld.Services.ReactionAPI.Services
{
  public class WatchHistoryService : IWatchHistoryService
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly string _movieServiceUrl;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    public WatchHistoryService(IUnitOfWork unitOfWork, IMapper mapper, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _httpClientFactory = httpClientFactory;
      _configuration = configuration;

      _movieServiceUrl = _configuration["ServiceUrls:MovieAPI"];
    }

    public async Task<bool> AddWatchHistoryAsync(string userId, WatchHistoryDTO watchHistory)
    {
      var entity = _mapper.Map<WatchHistory>(watchHistory);
      entity.UserId = userId;
      if (await _unitOfWork.WatchHistories.IsMovieWatchedAsync(entity.UserId, entity.MovieId, entity.EpisodeId))
      {
        _unitOfWork.WatchHistories.UpdateWatchHistory(entity);
      }
      else
      {
        _unitOfWork.WatchHistories.Add(entity);
      }
      return await _unitOfWork.CompleteAsync() > 0;
    }

    public async Task<bool> DeleteAllWatchHistoriesAsync(string userId)
    {
      var historiesOfUser = _unitOfWork.WatchHistories.Find(x => x.UserId == userId);
      _unitOfWork.WatchHistories.DeleteRange(historiesOfUser);
      return await _unitOfWork.CompleteAsync() > 0;
    }

    public async Task<bool> DeleteWatchHistoryAsync(string userId, int watchHistoryId)
    {
      var entity = _unitOfWork.WatchHistories.Find(p => p.UserId == userId && p.Id == watchHistoryId);
      _unitOfWork.WatchHistories.DeleteRange(entity);
      return await _unitOfWork.CompleteAsync() > 0;
    }

    public async Task<PagedList<WatchHistoryDTO>> GetHistoryByUserId(string userId, WatchHistoryParam reqParams)
    {
      PagedList<WatchHistory> historiesOfUser = await _unitOfWork.WatchHistories.GetFavoriteByUserId(userId, reqParams);
      List<int> movieIds = historiesOfUser.Records.Select(x => x.MovieId).Distinct().ToList();
      if (!movieIds.Any())
      {
        return new PagedList<WatchHistoryDTO>
        (
            new List<WatchHistoryDTO>(),
            0,
            reqParams.PageNumber,
            reqParams.PageSize
        );
      }
      var queryString = string.Join("&", movieIds.Select(id => $"ids={id}"));
      var finalUrl = $"{_movieServiceUrl}/api/movies/GetMoviesById?{queryString}";
      var client = _httpClientFactory.CreateClient();
      var response = await client.GetAsync(finalUrl);
      if (!response.IsSuccessStatusCode)
      {
        var errorMessage = await response.Content.ReadAsStringAsync();
        throw new Exception($"Failed to retrieve movie details from Movie Service. StatusCode: {response.StatusCode}, Error: {errorMessage}");
      }
      var responseContent = await response.Content.ReadAsStringAsync();
      MovieResponseDto movieResponse = JsonConvert.DeserializeObject<MovieResponseDto>(responseContent);
      var historiesOfUserDTO = _mapper.Map<List<WatchHistoryDTO>>(historiesOfUser.Records);
      foreach (var historyDTO in historiesOfUserDTO)
      {
        historyDTO.MovieName = movieResponse.Result.Where(p => p.MovieId == historyDTO.MovieId).Select(p => p.MovieName).FirstOrDefault();
        historyDTO.MovieImageUrl = movieResponse.Result.Where(p => p.MovieId == historyDTO.MovieId).Select(p => p.MovieImageUrl).FirstOrDefault();

      }
      return new PagedList<WatchHistoryDTO>(historiesOfUserDTO, historiesOfUser.TotalRecords, reqParams.PageNumber, reqParams.PageSize);

    }


  }
}
