using CineWorld.Services.MovieAPI.Models;
using System.ComponentModel;

namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public class MovieQueryParameters : BaseQueryParameters
  {
    // Filtering
    public string? Genre { get; set; }
    public int? CategoryId { get; set; }
    public string? Category { get; set; }
    public int? CountryId { get; set; }
    public string? Country { get; set; }
    public string? Name { get; set; }
    public string? Year { get; set; }
    public bool? IsHot { get; set; }
    public bool? Status { get; set; }
   
  }

}
