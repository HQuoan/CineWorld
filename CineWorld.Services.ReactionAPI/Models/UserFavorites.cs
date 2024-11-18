using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.ReactionAPI.Models
{
    public class UserFavorites
    {
        
        public string UserId { get; set; }
       
        public int MovieId { get; set; }
       
        public DateTime FavoritedAt { get; set; }
    }
}
