namespace CineWorld.Services.ViewAPI.Models.Dtos
{
  /// <summary>
  /// Represents pagination information for a list of items.
  /// </summary>
  public class PaginationDto
  {
    /// <summary>
    /// Gets or sets the current page number in the pagination.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages based on the total items and items per page.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets or sets the number of items displayed per page.
    /// </summary>
    public int TotalItemsPerPage { get; set; }

    /// <summary>
    /// Gets or sets the total number of items available (across all pages).
    /// </summary>
    public int TotalItems { get; set; }
  }
}
