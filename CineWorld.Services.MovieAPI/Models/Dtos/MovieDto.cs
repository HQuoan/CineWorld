using CineWorld.Services.MovieAPI.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  /// <summary>
  /// Represents a data transfer object for a Movie.
  /// </summary>
  public class MovieDto
  {
    /// <summary>
    /// Gets or sets the movie ID.
    /// </summary>
    public int MovieId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the category to which the movie belongs.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the country where the movie was produced.
    /// </summary>
    public int? CountryId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the series that the movie is a part of. This field is optional.
    /// </summary>
    [DefaultValue(null)]
    public int? SeriesId { get; set; }

    /// <summary>
    /// Gets or sets the name of the movie. This field is required.
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the slug of the movie, typically used for URL-friendly representation.
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Gets or sets the original name of the movie, if different from the standard name.
    /// </summary>
    public string? OriginName { get; set; }

    /// <summary>
    /// Gets or sets the current episode number in a series, if applicable.
    /// </summary>
    public int? EpisodeCurrent { get; set; }

    /// <summary>
    /// Gets or sets the total number of episodes in a series, if applicable.
    /// </summary>
    public int? EpisodeTotal { get; set; }

    /// <summary>
    /// Gets or sets the duration of the movie.
    /// </summary>
    public string? Duration { get; set; }

    /// <summary>
    /// Gets or sets a description of the movie.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the image URL for the movie.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the trailer URL for the movie.
    /// </summary>
    public string? Trailer { get; set; }

    /// <summary>
    /// Gets or sets the year the movie was released.
    /// </summary>
    public int? Year { get; set; }

    /// <summary>
    /// Gets or sets the view count of the movie. Defaults to 0.
    /// </summary>
    public int View { get; set; } = 0;

    /// <summary>
    /// Gets or sets the showtimes for the movie. This field is optional.
    /// </summary>
    [DefaultValue(null)]
    public string? ShowTimes { get; set; }

    /// <summary>
    /// Gets or sets the detailed showtimes for the movie, including days of the week. This field is optional.
    /// </summary>
    [DayOfWeekValidation]
    [DefaultValue(null)]
    public string? ShowTimesDetails { get; set; }

    /// <summary>
    /// Gets or sets a list of actors featured in the movie.
    /// </summary>
    [DefaultValue(null)]
    public string? Actors { get; set; }

    /// <summary>
    /// Gets or sets whether the movie is marked as "Hot" or trending.
    /// </summary>
    public bool? IsHot { get; set; }

    /// <summary>
    /// Gets or sets the status of the movie (active or inactive). Defaults to true (active).
    /// </summary>
    public bool? Status { get; set; } = true;

    /// <summary>
    /// Gets or sets the date and time when the movie was created.
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the movie was last updated.
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets a list of genre IDs associated with the movie.
    /// </summary>
    public List<int> GenreIds { get; set; } = new List<int>();
  }
}
