using CineWorld.Services.EpisodeAPI.Models.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.EpisodeAPI.Models
{
  public class Episode
  {
    [Key]
    public int EpisodeId { get; set; }
    [Required]
    public int MovieId { get; set; }
    [NotMapped]
    public MovieDto? Movie { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "EpisodeNumber phải lớn hơn 0.")]
    public int EpisodeNumber { get; set; }
    public bool IsFree { get; set; } = false;
    public bool Status { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    public ICollection<Server> Servers { get; set; }

  }
}
