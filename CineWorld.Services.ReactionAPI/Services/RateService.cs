using AutoMapper;
using CineWorld.Services.ReactionAPI.Models.Dtos.UserRate;
using CineWorld.Services.ReactionAPI.Models.Entities;
using CineWorld.Services.ReactionAPI.Repositories.Interface;
using CineWorld.Services.ReactionAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.ReactionAPI.Services
{
    public class RateService : IRateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public RateService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<bool> AddRateAsync(string userId, UserRateDTO rating)
        {
            var rateEntity = _mapper.Map<UserRate>(rating);
            rateEntity.UserId = userId;
            _unitOfWork.UserRates.Add(rateEntity);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public ResponseRatingDTO GetAverageRatingAsync(int movieId)
        {
            var ratings = _unitOfWork.UserRates.GetRatingsByMovieId(movieId);
            return new ResponseRatingDTO
            {
                RatingCount = (!ratings.Any()) ? 0 : ratings.Count(),
                AverageRating = (!ratings.Any()) ? 0 : ratings.Average(r => r.RatingValue),
            };

        }

        public async Task<UserRateDTO> GetRateAsync(int movieId, string userId)
        {
            var rateEntity = await _unitOfWork.UserRates.GetRateAsync(movieId, userId);
            var userRateDto = _mapper.Map<UserRateDTO>(rateEntity);
            
            return userRateDto;

        }

        public async Task<bool> UpdateRateAsync(string userId, UserRateDTO rating)
        {

            var rateEntity = _mapper.Map<UserRate>(rating);
            rateEntity.UserId = userId;
            _unitOfWork.UserRates.Update(rateEntity);
            return await _unitOfWork.CompleteAsync() > 0;
        }
       

    }
}