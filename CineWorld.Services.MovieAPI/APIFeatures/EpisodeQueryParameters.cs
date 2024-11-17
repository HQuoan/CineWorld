
namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public class EpisodeQueryParameters : BaseQueryParameters
  {
    public string? Name { get; set; }
    public int? MovieId { get; set; }
    public int? EpisodeNumber { get; set; }
    public bool? IsFree { get; set; }
    public bool? Status { get; set; } = true;

    /// <summary>
    /// Specifies the property name for sorting. 
    /// Valid values: "EpisodeId, Name, EpisodeNumber, Status".
    /// Use "-" prefix for descending order (e.g., "-Name" for descending by Name).
    /// </summary>
    public new string? OrderBy { get; set; }
  }

}
