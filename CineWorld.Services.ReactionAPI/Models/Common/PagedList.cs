namespace CineWorld.Services.ReactionAPI.Models.Common
{
    public class PagedList<T>
    {
        public List<T> Records { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public PagedList(List<T> records, int totalRecords, int currentPage, int pageSize)
        {
            Records = records;
            TotalRecords = totalRecords;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);
        }
    }
}
