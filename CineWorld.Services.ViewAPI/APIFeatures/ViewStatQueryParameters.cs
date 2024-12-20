
using System.ComponentModel;

namespace CineWorld.Services.ViewAPI.APIFeatures
{
  public class ViewStatQueryParameters: BaseQueryParameters
  {
    [DefaultValue("Movie")]
    public string StatWith { get; set; } = "Movie";
    public int TopMovies { get; set; } = 10;
    public DateTime From { get; set; } = DateTime.MinValue;
    public DateTime To { get; set; } = DateTime.UtcNow;
    public bool IsAscending { get; set; } = false;
    public bool WithMovieInformation { get; set; } = false;
    public new int? PageSize { get; set; } = null;

  }
}
