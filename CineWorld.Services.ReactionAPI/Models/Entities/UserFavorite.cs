using CineWorld.Services.ReactionAPI.Constants;
using CineWorld.Services.ReactionAPI.Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.ReactionAPI.Models.Entities
{
    [Table(TableName.USER_FAVORITE)]
    public class UserFavorite : EntityBase
    {
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("movie_id")]
        public int MovieId { get; set; }
        
    }
}
