using System.ComponentModel;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.APIFeatures
{
  /// <summary>
  /// Base query parameters for filtering, sorting, and pagination.
  /// </summary>
  public class BaseQueryParameters
  {
    // public List<Expression<Func<T, bool>>> Filters { get; set; } = new List<Expression<Func<T, bool>>>();

    /// <summary>
    /// Specifies the property name for sorting. 
    /// Use "-" prefix for descending order (e.g., "-Name" for descending by Name).
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Specifies related entities to include in the query.
    /// Comma-separated list of entity names to include.
    /// </summary>
    public string? IncludeProperties { get; set; }

    private int _pageNumber = 1;

    /// <summary>
    /// The page number for pagination. Defaults to 1. 
    /// Values less than 1 will be set to 1.
    /// </summary>
    /// 
    [DefaultValue(1)]
    public int PageNumber
    {
      get => _pageNumber;
      set => _pageNumber = (value < 1) ? 1 : value;
    }

    private int _pageSize = 25;
    private const int MaxPageSize = 100;

    /// <summary>
    /// The number of items per page for pagination. 
    /// Default is 25, with a maximum of 100 items per page.
    /// </summary>
    /// 
    [DefaultValue(25)]
    public int PageSize
    {
      get => _pageSize;
      set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
  }


}
