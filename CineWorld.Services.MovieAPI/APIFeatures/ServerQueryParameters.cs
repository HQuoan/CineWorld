
namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public class ServerQueryParameters : BaseQueryParameters
  {
    public string? Name { get; set; }
    public int? EpisodeId { get; set; }

    /// <summary>
    /// Specifies the property name for sorting. 
    /// Valid values: "ServerId, Name".
    /// Use "-" prefix for descending order (e.g., "-Name" for descending by Name).
    /// </summary>
    public new string? OrderBy { get; set; }
  }

}
