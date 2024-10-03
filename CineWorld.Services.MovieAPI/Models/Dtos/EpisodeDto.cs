
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.MovieAPI.Models.Dtos
{
  public class EpisodeDto
  {
    public int EpisodeId { get; set; }
    public int MovieId { get; set; }
    public string Name { get; set; }
    [DefaultValue(1)]
    [Range(1, int.MaxValue, ErrorMessage = "EpisodeNumber phải lớn hơn 0.")]
    public int EpisodeNumber { get; set; }
    [DefaultValue(false)]
    public bool IsFree { get; set; }
    public bool Status { get; set; } = true;
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
  }
}
