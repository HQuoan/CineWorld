namespace CineWorld.Services.ReactionAPI.Models.Dtos.WatchHistory
{
    public class WatchHistoryDTO
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string? MovieName { get; set; }
        public string? MovieImageUrl { get; set; }
        public string EpisodeName { get; set; }

        public int EpisodeId { get; set; }

        public TimeSpan WatchedDuration { get; set; }

        public DateTime LastWatched { get; set; }
    }
}
