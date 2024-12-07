namespace CineWorld.Services.HistoryAPI.Models.Dtos
{
    public class WatchHistoryDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public int? EpisodeId { get; set; }
        public string? MovieUrl { get; set; }
        public TimeSpan WatchedDuration { get; set; }
        public DateTime LastWatched { get; set; }
    }
}
