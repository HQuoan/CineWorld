namespace CineWorld.Services.ReactionAPI.Models.Dtos
{
    public class UserFavoritesDto
    {
        public string UserId { get; set; }

        public int MovieId { get; set; }

        public DateTime FavoritedAt { get; set; }
    }
}
