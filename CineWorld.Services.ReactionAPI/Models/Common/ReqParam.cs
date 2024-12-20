using CineWorld.Services.ReactionAPI.Constants;

namespace CineWorld.Services.ReactionAPI.Models.Common
{
    public abstract class ReqParam
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;
        
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = Math.Max(value, PaginationConfig.MinPageNumber);

        }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Min(value, PaginationConfig.MaxPageSize);
        }
    }
}
