namespace CineWorld.Services.AuthAPI.Models.Dto
{
  /// <summary>
  /// Data transfer object representing the response to a user login request.
  /// </summary>
  public class LoginResponseDto
  {
    /// <summary>
    /// Information about the authenticated user.
    /// </summary>
    public UserDto User { get; set; }

    /// <summary>
    /// The JWT token generated for the user upon successful login.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// An optional message providing additional context about the login process (e.g., errors or warnings).
    /// </summary>
    public string? Message { get; set; }
  }
}
