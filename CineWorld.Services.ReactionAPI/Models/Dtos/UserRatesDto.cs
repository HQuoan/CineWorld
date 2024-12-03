namespace CineWorld.Services.ReactionAPI.Models.Dtos
{
    public class UserRatesDto
    {
        public int RatingId { get; set; }
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public double RatingValue { get; set; }
        public DateTime RatedAt { get; set; }

    }
}
