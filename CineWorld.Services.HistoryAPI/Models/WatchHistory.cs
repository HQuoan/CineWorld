using System.ComponentModel.DataAnnotations;

namespace CineWorld.Services.HistoryAPI.Models
{
    public class WatchHistory
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public string EpisodeName { get; set; }
        public int EpisodeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeSpan WatchedDuration { get; set; }
        public DateTime LastWatched { get; set; }
    }
}
