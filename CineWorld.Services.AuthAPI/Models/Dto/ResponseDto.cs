namespace CineWorld.Services.AuthAPI.Models.Dto
{
  /// <summary>
  /// Represents a standardized response object for API responses.
  /// </summary>
  public class ResponseDto
  {
    /// <summary>
    /// The total number of items in the response (used for pagination).
    /// </summary>
    public int? TotalItems { get; set; }

    /// <summary>
    /// The result of the API request, can be any object (e.g., list of data, user info, etc.).
    /// </summary>
    public object? Result { get; set; }

    /// <summary>
    /// Indicates if the request was successful.
    /// </summary>
    public bool IsSuccess { get; set; } = true;

    /// <summary>
    /// A message providing additional details about the result (e.g., error or success message).
    /// </summary>
    public string Message { get; set; } = "";
  }
}
