
namespace CineWorld.Services.ViewAPI.APIFeatures
{
  public class ViewQueryParameters : BaseQueryParameters
  {
    public string? IpAddress { get; set; }
    public string? UserId { get; set; }
    public int? MovieId { get; set; }
    public int? EpisodeId { get; set; }
    public DateTime From { get; set; } = DateTime.MinValue;
    public DateTime To { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Specifies the property name for sorting. 
    /// Valid values: "IpAddree, UserId, MovieId, EpisodeId, ViewDate".
    /// Use "-" prefix for descending order (e.g., "-Name" for descending by Name).
    /// </summary>
    public new string? OrderBy { get; set; }
    public bool WithMovieInformation { get; set; } = false; 

   }

}
