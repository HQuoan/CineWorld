using CineWorld.Services.MovieAPI.Models;
using System.ComponentModel;

namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public class MovieQueryParameters : BaseQueryParameters
  {
    /// <summary>
    /// Filters movies by genre.
    /// </summary>
    public string? Genre { get; set; }

    /// <summary>
    /// Filters movies by category ID.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Filters movies by category name.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Filters movies by country ID.
    /// </summary>
    public int? CountryId { get; set; }

    /// <summary>
    /// Filters movies by country name.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Filters movies by name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filters movies by release year.
    /// </summary>
    public string? Year { get; set; }

    /// <summary>
    /// Filters movies that are marked as "hot".
    /// </summary>
    public bool? IsHot { get; set; }

    /// <summary>
    /// Filters movies by status.
    /// </summary>
    public bool? Status { get; set; }

    /// <summary>
    /// Filters movies by status. 
    /// Valid values: "Mon, Tue, Wed, Thu, Fri, Sat, Sun"
    /// </summary>
    public string? ShowTimes { get; set; }


    /// <summary>
    /// Specifies the property name for sorting. 
    /// Valid values: "MovieId, Name, Year, View, CreatedDate, UpdatedDate, IsHot, Status".
    /// Use "-" prefix for descending order (e.g., "-Name" for descending by Name).
    /// </summary>
    public new string? OrderBy { get; set; }
  }


}
