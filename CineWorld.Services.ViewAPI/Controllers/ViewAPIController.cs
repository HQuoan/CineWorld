using AutoMapper;
using CineWorld.Services.ViewAPI.APIFeatures;
using CineWorld.Services.ViewAPI.Exceptions;
using CineWorld.Services.ViewAPI.Models;
using CineWorld.Services.ViewAPI.Models.Dtos;
using CineWorld.Services.ViewAPI.Repositories.IRepositories;
using CineWorld.Services.ViewAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace CineWorld.Services.ViewAPI.Controllers
{
  [Route("api/views")]
  [ApiController]
  public class ViewAPIController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMovieService _movieService;

    private readonly ResponseDto _response;
    public ViewAPIController(IUnitOfWork unitOfWork, IMapper mapper, IMovieService movieService)
    {
      _response = new ResponseDto();
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _movieService = movieService;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] ViewQueryParameters queryParameters)
    {
      var query = ViewFeatures.Build(queryParameters);
      IEnumerable<View> views = await _unitOfWork.View.GetAllAsync(query);

      List<GetEpsiodeWithMovieInformationDto> movies = new List<GetEpsiodeWithMovieInformationDto>();

      if (queryParameters.WithMovieInformation == true)
      {
        List<int> episodeIds = views.Select(c => c.EpisodeId).Distinct().ToList();
        movies = await _movieService.GetEpsiodeWithMovieInformatio(new GetEpsiodeWithMovieInformationRequestDto { EpisodeIds = episodeIds });
      }

      // Tạo Dictionary để tối ưu việc tìm kiếm movie thông qua EpisodeId
      var movieDictionary = movies.ToDictionary(m => m.EpisodeId);

      // Duyệt qua từng view và thêm thông tin movie vào ViewDto
      var viewDtos = _mapper.Map<IEnumerable<ViewDto>>(views).ToList();

      foreach (var viewDto in viewDtos)
      {
        // Kiểm tra và lấy thông tin movie từ Dictionary
        if (movieDictionary.TryGetValue(viewDto.EpisodeId, out var movieInfo))
        {
          viewDto.MovieInfor = movieInfo;
        }
      }

      // Gắn thông tin ánh xạ vào _response
      _response.Result = viewDtos;

      int totalItems = await _unitOfWork.View.CountAsync();
      _response.Pagination = new PaginationDto
      {
        TotalItems = totalItems,
        TotalItemsPerPage = queryParameters.PageSize,
        CurrentPage = queryParameters.PageNumber,
        TotalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize)
      };

      return Ok(_response);
    }


    [HttpPost]
    [EnableRateLimiting("IpRateLimit")]
    public async Task<ActionResult<ResponseDto>> Post([FromQuery] AddViewRequestDto model) {

      var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
      var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                 ?? HttpContext.Connection.RemoteIpAddress?.ToString();

      //if( ip == null && userId == null )
      //{
      //  throw new NotFoundException("Can't find IpAddress, please try again or login your account!");
      //} 

      View view = new View
      {
        IpAddress = ip,
        UserId = userId,
        MovieId = model.MovieId,
        EpisodeId = model.EpisodeId,
        ViewDate = DateTime.UtcNow,
      };


      await _unitOfWork.View.AddAsync(view);
      await _unitOfWork.SaveAsync();

      _response.Result = view;
      return Created(string.Empty, _response);
    }

    [HttpPut]
    public async Task<ActionResult<ResponseDto>> Put([FromBody] ViewDto viewDto)
    {
      View view = _mapper.Map<View>(viewDto);

      View viewFromDb = await _unitOfWork.View.GetAsync(c => c.ViewId == viewDto.ViewId);
      if (viewFromDb == null)
      {
        throw new NotFoundException($"View with ID: {viewDto.ViewId} not found.");
      }

      await _unitOfWork.View.UpdateAsync(view);
      await _unitOfWork.SaveAsync();

      _response.Result = _mapper.Map<ViewDto>(view);

      return Ok(_response);
    }

    /// <summary>
    /// Deletes a view by its ID.
    /// </summary>
    /// <param name="id">The ID of the view to delete.</param>
    /// <returns>A no content response if successful.</returns>
    /// <exception cref="NotFoundException">Thrown if no view is found with the given ID.</exception>
    [HttpDelete]
    public async Task<ActionResult<ResponseDto>> Delete(int id)
    {
      var view = await _unitOfWork.View.GetAsync(c => c.ViewId == id);
      if (view == null)
      {
        throw new NotFoundException($"View with ID: {id} not found.");
      }

      await _unitOfWork.View.RemoveAsync(view);
      await _unitOfWork.SaveAsync();

      return NoContent();
    }
  }
}
