namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  /// <summary>
  /// Represents the response structure for API responses.
  /// This class contains information about the result, success status, message, and pagination.
  /// </summary>
  public class ResponseDto
  {
    /// <summary>
    /// Gets or sets the pagination information if applicable.
    /// </summary>
    public PaginationDto? Pagination { get; set; }

    /// <summary>
    /// Gets or sets the result of the API request. This can be any object representing the data returned.
    /// </summary>
    public object? Result { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the API request was successful.
    /// </summary>
    public bool IsSuccess { get; set; } = true;

    /// <summary>
    /// Gets or sets a message related to the request. This can provide additional information about the result.
    /// </summary>
    public string Message { get; set; } = "";
  }
}
