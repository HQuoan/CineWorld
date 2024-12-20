using CineWorld.Services.ReactionAPI.Constants;
using CineWorld.Services.ReactionAPI.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.ReactionAPI.Models.Entities
{
    [Table(TableName.WATCH_HISTORY)]
    public class WatchHistory : EntityBase
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("movie_id")]
        public int MovieId { get; set; }
        [Column("episode_name")]
        public string EpisodeName { get; set; }
        [Column("episode_id")]
        public int EpisodeId { get; set; }
        [Column("watched_duration")]
        public TimeSpan WatchedDuration { get; set; }
        [Column("last_watched")]
        public DateTime LastWatched { get; set; }


    }
}
