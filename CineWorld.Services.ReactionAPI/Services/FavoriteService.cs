using AutoMapper;
using CineWorld.Services.ReactionAPI.Models.Common;
using CineWorld.Services.ReactionAPI.Models.Dtos;
using CineWorld.Services.ReactionAPI.Models.Dtos.UserFavorite;
using CineWorld.Services.ReactionAPI.Models.Entities;
using CineWorld.Services.ReactionAPI.Models.ReqParams;
using CineWorld.Services.ReactionAPI.Repositories.Interface;
using CineWorld.Services.ReactionAPI.Services.Interface;
using Newtonsoft.Json;

namespace CineWorld.Services.ReactionAPI.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly string _movieServiceUrl = "https://localhost:7001/api/movies/GetMoviesById";
        private readonly IHttpClientFactory _httpClientFactory;
        public FavoriteService(IUnitOfWork unitOfWork, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> AddFavoriteAsync(string userId, UserFavoriteDTO userFavorite)
        {
            var entityFavorite = _mapper.Map<UserFavorite>(userFavorite);
            entityFavorite.UserId = userId;
            _unitOfWork.UserFavorites.Add(entityFavorite);
            return await _unitOfWork.CompleteAsync() > 0;

        }

        public async Task<bool> CheckFavoriteAsync(string userId, int movieId)
        {
            return await _unitOfWork.UserFavorites.CheckFavoriteAsync(userId, movieId);
        }

        public async Task<PagedList<ResponseFavoriteDTO>> GetFavoriteByUserId(string userId, FavoriteParam reqParams)
        {
            PagedList<UserFavorite> entities = await _unitOfWork.UserFavorites.GetFavoriteByUserId(userId, reqParams);
            List<int> moviesId = new List<int>();
            foreach (var entity in entities.Records)
            {
                moviesId.Add(entity.MovieId);
            }

            if (!moviesId.Any())
            {
                return new PagedList<ResponseFavoriteDTO>
                (
                    new List<ResponseFavoriteDTO>(),
                    0,
                    reqParams.PageNumber,
                    reqParams.PageSize
                );
            }
            var queryString = string.Join("&", moviesId.Select(id => $"ids={id}"));
            var finalUrl = $"{_movieServiceUrl}?{queryString}";
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(finalUrl);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve movie details from Movie Service. StatusCode: {response.StatusCode}, Error: {errorMessage}");
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseContent))
            {
                throw new Exception("The response content is empty or null.");
            }
            MovieResponseDto movieResponse = JsonConvert.DeserializeObject<MovieResponseDto>(responseContent);
            return new PagedList<ResponseFavoriteDTO>(movieResponse.Result, entities.TotalRecords, entities.CurrentPage, entities.PageSize);

        }
        public async Task<bool> RemoveFavoriteAsync(string userId, UserFavoriteDTO userFavorite)
        {
            var entityFavorite = _mapper.Map<UserFavorite>(userFavorite);
            entityFavorite.UserId = userId;
            _unitOfWork.UserFavorites.Delete(entityFavorite);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
