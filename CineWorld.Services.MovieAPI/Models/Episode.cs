using CineWorld.Services.MovieAPI.Models.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.MovieAPI.Models
{
  public class Episode
  {
    [Key]
    public int EpisodeId { get; set; }
    [Required]
    public int MovieId { get; set; }
    [ForeignKey(nameof(MovieId))]
    public Movie? Movie { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "EpisodeNumber phải lớn hơn 0.")]
    public int EpisodeNumber { get; set; }
    public bool IsFree { get; set; } = false;
    public bool Status { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    public IEnumerable<Server> Servers { get; set; }

  }
}
