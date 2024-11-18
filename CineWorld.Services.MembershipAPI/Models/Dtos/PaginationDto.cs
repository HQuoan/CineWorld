namespace CineWorld.Services.MembershipAPI.Models.Dtos
{
  public class PaginationDto
  {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItemsPerPage { get; set; }
        public int TotalItems { get; set; }
    }
}
