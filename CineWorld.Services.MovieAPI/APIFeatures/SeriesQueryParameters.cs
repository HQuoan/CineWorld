
namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public class SeriesQueryParameters : BaseQueryParameters
  {
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public bool? Status { get; set; } = true;

    /// <summary>
    /// Specifies the property name for sorting. 
    /// Valid values: "SeriesId, Name, Slug, Status".
    /// Use "-" prefix for descending order (e.g., "-Name" for descending by Name).
    /// </summary>
    public new string? OrderBy { get; set; }
  }

}
