using System.ComponentModel;

namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public class MovieQueryParameters
  {
    // Filtering
    public string? Genre { get; set; }
    public int? CategoryId { get; set; }
    public int? CountryId { get; set; }
    public string? Name { get; set; }
    public string? Year { get; set; }
    public bool? IsHot { get; set; }
    public bool? Status { get; set; }

    public string? IncludeProperties {  get; set; }

    // Sorting
    public string? OrderBy { get; set; } = "Name"; // Giá trị mặc định

    // Pagination
    [DefaultValue(1)]
    private int _pageNumber = 1;
    public int PageNumber
    {
      get => _pageNumber;
      set => _pageNumber = (value < 1) ? 1 : value;
    }
    [DefaultValue(10)]
    private int _pageSize = 10;
    private const int MaxPageSize = 100;
    public int PageSize
    {
      get => _pageSize;
      set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
  }

}
