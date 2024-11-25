namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  /// <summary>
  /// Represents pagination details for API responses, including current page, total pages, total items per page, and total items.
  /// </summary>
  public class PaginationDto
  {
    /// <summary>
    /// Gets or sets the current page number in the paginated response.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages in the paginated response.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets or sets the total number of items per page in the paginated response.
    /// </summary>
    public int TotalItemsPerPage { get; set; }

    /// <summary>
    /// Gets or sets the total number of items in the entire dataset.
    /// </summary>
    public int TotalItems { get; set; }
  }
}
