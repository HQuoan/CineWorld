namespace CineWorld.Services.ReactionAPI.Models.Dtos
{
    public class ResponseDTO
    {
        //public object? Pagination { get; set; }
        public object? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "Success";
    }
}
