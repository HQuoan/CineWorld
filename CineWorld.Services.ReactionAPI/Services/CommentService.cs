using AutoMapper;
using CineWorld.Services.ReactionAPI.Constants;
using CineWorld.Services.ReactionAPI.Models.Common;
using CineWorld.Services.ReactionAPI.Models.Dto;
using CineWorld.Services.ReactionAPI.Models.Dtos;
using CineWorld.Services.ReactionAPI.Models.Dtos.Comment;
using CineWorld.Services.ReactionAPI.Models.Dtos.UserFavorite;
using CineWorld.Services.ReactionAPI.Models.Entities;
using CineWorld.Services.ReactionAPI.Models.ReqParams;
using CineWorld.Services.ReactionAPI.Repositories.Interface;
using CineWorld.Services.ReactionAPI.Services.Interface;
using Newtonsoft.Json;

namespace CineWorld.Services.ReactionAPI.Services
{
  public class CommentService : ICommentService
  {
    private readonly IUnitOfWork _unitOfWork;
    private IMapper _mapper;
    private readonly string _userServiceUrl;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    public CommentService(IUnitOfWork unitOfWork, IMapper mapper, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _httpClientFactory = httpClientFactory;
      _configuration = configuration;
      _userServiceUrl = _configuration["ServiceUrls:AuthAPI"];
    }
    public async Task<bool> AddCommentAsync(string userId, CreateCommentDTO comment)
    {
      var entityComment = _mapper.Map<Comment>(comment);
      entityComment.UserId = userId;
      _unitOfWork.Comments.Add(entityComment);
      return await _unitOfWork.CompleteAsync() > 0;
    }

    public async Task<bool> DeleteCommentAsync(string role, string userId, int commentId)
    {
      var entity = await _unitOfWork.Comments.GetByIdAsync(commentId);

      if (entity == null)
      {
        return false;
      }
      if (role != SD.AdminRole && entity.UserId != userId)
      {
        return false;
      }
      _unitOfWork.Comments.Delete(entity);

      return await _unitOfWork.CompleteAsync() > 0;
    }
    public Task<PagedList<CommentDTO>> GetCommentByMovieIdAsync(string userId, CommentParam reqParams)
    {
      throw new NotImplementedException();
    }

    public async Task<bool> UpdateCommentAsync(string role, string userId, CreateCommentDTO commentDto)
    {
      Comment entity = await _unitOfWork.Comments.GetByIdAsync(commentDto.CommentId);
      entity.CommentContent = commentDto.CommentContent;
      if (entity == null)
      {
        return false;
      }
      if (role != SD.AdminRole && entity.UserId != userId)
      {
        return false;
      }
      _unitOfWork.Comments.Update(entity);
      return await _unitOfWork.CompleteAsync() > 0;
    }
    public async Task<PagedList<CommentDTO>> GetCommentByMovieIdAsync(int movieId, CommentParam reqParams)
    {
      PagedList<Comment> entities = await _unitOfWork.Comments.GetCommentByMovieId(movieId, reqParams);
      List<string> userId = new List<string>();
      foreach (var entity in entities.Records)
      {
        userId.Add(entity.UserId);
      }

      if (!userId.Any())
      {
        return new PagedList<CommentDTO>
        (
            new List<CommentDTO>(),
            0,
            reqParams.PageNumber,
            reqParams.PageSize
        );
      }
      var queryString = string.Join("&", userId.Select(id => $"ids={id}"));
      var finalUrl = $"{_userServiceUrl}/api/users/GetUserInformationById?{queryString}";
      var client = _httpClientFactory.CreateClient();
      var response = await client.GetAsync(finalUrl);
      Console.WriteLine($"API Response: {response}");
      if (!response.IsSuccessStatusCode)
      {
        var errorMessage = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Error: {errorMessage}");
        throw new Exception($"Failed to retrieve movie details from Movie Service. StatusCode: {response.StatusCode}, Error: {errorMessage}");
      }
      var responseContent = await response.Content.ReadAsStringAsync();
      if (string.IsNullOrEmpty(responseContent))
      {
        throw new Exception("The response content is empty or null.");
      }
      UserResponseDto userResponse = JsonConvert.DeserializeObject<UserResponseDto>(responseContent);
      var commentDTOs = _mapper.Map<List<CommentDTO>>(entities.Records);

      foreach (var comment in commentDTOs)
      {
        var userInfo = userResponse.Result.FirstOrDefault(p => p.Id == comment.UserId);
        if (userInfo != null)
        {
          comment.FullName = userInfo.FullName;
          comment.Avatar = userInfo.Avatar;
        }
      }
      foreach (var comment in commentDTOs)
      {
        comment.UserId = null;
      }


      return new PagedList<CommentDTO>(
          commentDTOs,
          entities.TotalRecords,
          entities.CurrentPage,
          entities.PageSize
      );

    }
  }
}
