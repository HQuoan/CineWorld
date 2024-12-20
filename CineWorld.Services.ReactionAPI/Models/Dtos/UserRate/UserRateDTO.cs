namespace CineWorld.Services.ReactionAPI.Models.Dtos.UserRate
{
    public class UserRateDTO
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public double RatingValue { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
