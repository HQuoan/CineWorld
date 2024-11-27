namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  /// <summary>
  /// Represents a standardized response structure for API responses, including pagination, result data, success status, and messages.
  /// </summary>
  public class ResponseDto
  {
    /// <summary>
    /// Gets or sets the pagination details if the response includes paginated data.
    /// </summary>
    public PaginationDto? Pagination { get; set; }

    /// <summary>
    /// Gets or sets the result of the operation, which can be any object representing the data returned by the API.
    /// </summary>
    public object? Result { get; set; }

    /// <summary>
    /// Gets or sets a boolean value indicating whether the operation was successful.
    /// Default is <c>true</c>.
    /// </summary>
    public bool IsSuccess { get; set; } = true;

    /// <summary>
    /// Gets or sets the message providing additional details about the result of the operation. 
    /// It can contain error or success information.
    /// Default is an empty string.
    /// </summary>
    public string Message { get; set; } = "";
  }
}
