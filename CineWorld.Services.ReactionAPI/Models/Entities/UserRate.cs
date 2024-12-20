using CineWorld.Services.ReactionAPI.Constants;
using CineWorld.Services.ReactionAPI.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.ReactionAPI.Models.Entities
{
    [Table(TableName.USER_RATE)]
    public class UserRate : EntityBase
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("movie_id")]
        public int MovieId { get; set; }
        [Column("rating_value")]
        public double RatingValue { get; set; }
        

    }
}
