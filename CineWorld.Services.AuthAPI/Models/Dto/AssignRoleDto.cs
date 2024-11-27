namespace CineWorld.Services.AuthAPI.Models.Dto
{
  /// <summary>
  /// Data transfer object for assigning a role to a user.
  /// </summary>
  public class AssignRoleDto
  {
    /// <summary>
    /// The email address of the user to whom the role will be assigned.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The role to assign to the user. It should match the predefined roles in the system.
    /// </summary>
    public string Role { get; set; }
  }
}
