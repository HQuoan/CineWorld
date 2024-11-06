using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public class BaseQueryParameters
  {
    //public List<Expression<Func<T, bool>>> Filters { get; set; } = new List<Expression<Func<T, bool>>>();
    public string? OrderBy { get; set; }
    public string? IncludeProperties { get; set; }
    private int _pageNumber = 1;
    public int PageNumber
    {
      get => _pageNumber;
      set => _pageNumber = (value < 1) ? 1 : value;
    }
    private int _pageSize = 20;
    private const int MaxPageSize = 100;
    public int PageSize
    {
      get => _pageSize;
      set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
  }

}
