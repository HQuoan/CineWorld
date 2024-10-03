using CineWorld.Services.MovieAPI.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.MovieAPI.Models
{
  public class Server
  {
    [Key]
    public int ServerId { get; set; }
    [Required]
    public int EpisodeId { get; set; }
    [ForeignKey(nameof(EpisodeId))]
    public Episode Episode { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Link { get; set; }
  }
}
