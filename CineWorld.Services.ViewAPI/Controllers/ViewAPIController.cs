using AutoMapper;
using CineWorld.Services.ViewAPI.APIFeatures;
using CineWorld.Services.ViewAPI.Exceptions;
using CineWorld.Services.ViewAPI.Models;
using CineWorld.Services.ViewAPI.Models.Dtos;
using CineWorld.Services.ViewAPI.Repositories.IRepositories;
using CineWorld.Services.ViewAPI.Services.IService;
using CineWorld.Services.ViewAPI.Utilities;
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
    private readonly IConfiguration _configuration;

    private readonly ResponseDto _response;
    public ViewAPIController(IUnitOfWork unitOfWork, IMapper mapper, IMovieService movieService, IConfiguration configuration)
    {
      _response = new ResponseDto();
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _movieService = movieService;
      _configuration = configuration;
    }

    [HttpGet]
    [Route("ViewStat")]
    public async Task<ActionResult<ResponseDto>> ViewStat([FromQuery] ViewStatQueryParameters queryParameters)
    {
      var query = ViewStatFeatures.Build(queryParameters);
      IEnumerable<View> views = await _unitOfWork.View.GetAllAsync(query);

      var totalItems = views.Count();

      IEnumerable<ViewStatResultDto> viewStats;
      string statWith = queryParameters.StatWith.ToLower();

      // Thống kê theo phim hoặc tập
      if (statWith == "movie")
      {
        // Thống kê theo từng phim (gộp theo MovieId)
        viewStats = views
            .GroupBy(v => v.MovieId)
            .Select(g => new ViewStatResultDto
            {
              Key = g.Key,  // MovieId
              ViewCount = g.Count()  // Đếm số lượng view
            });
      }
      else if (statWith == "episode")
      {
        // Thống kê theo từng tập (gộp theo EpisodeId)
        viewStats = views
            .GroupBy(v => v.EpisodeId)
            .Select(g => new ViewStatResultDto
            {
              Key = g.Key,  // EpisodeId
              ViewCount = g.Count()  // Đếm số lượng view
            });
      }
      else
      {
        return BadRequest("Invalid StatWith value. It must be 'movie' or 'episode'.");
      }

      // Sắp xếp theo ViewCount
      if (queryParameters.IsAscending)
      {
        viewStats = viewStats.OrderBy(c => c.ViewCount);
      }
      else
      {
        viewStats = viewStats.OrderByDescending(c => c.ViewCount);
      }

      viewStats = viewStats.Take(queryParameters.TopMovies);

      var moviesInfor = new Dictionary<int, MovieInforDto>();
      var episodesInfor = new Dictionary<int, EpisodeInforDto>();


      if (statWith == "movie")
      {
        List<int> ids = views.Select(c => c.MovieId).ToList();
        var resutl = await _movieService.GetMoviesInfor(new IdsRequestDto { Ids = ids });
        moviesInfor = resutl.ToDictionary(m => m.MovieId);
      }
      else if (statWith == "episode")
      {
        List<int> ids = views.Select(c => c.EpisodeId).ToList();
        var resutl = await _movieService.GetEpsiodesInfor(new IdsRequestDto { Ids = ids });
        episodesInfor = resutl.ToDictionary(m => m.EpisodeId);
      }


      viewStats = viewStats.ToList();

      foreach (var viewDto in viewStats)
      {
        if (statWith == "movie")
        {
          if (moviesInfor.TryGetValue(viewDto.Key, out var movieInfo))
          {
            viewDto.MovieInfor = movieInfo;
          }
        }
        else if (statWith == "episode")
        {
          if (episodesInfor.TryGetValue(viewDto.Key, out var episodeInfor))
          {
            viewDto.EpisodeInfor = episodeInfor;
          }
        }
      }


      // Gắn thông tin ánh xạ vào _response
      _response.Result = viewStats;

      _response.Pagination = new PaginationDto
      {
        TotalItems = totalItems,
      };

      return Ok(_response);
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDto>> Get([FromQuery] ViewQueryParameters queryParameters)
    {
      var query = ViewFeatures.Build(queryParameters);
      IEnumerable<View> views = await _unitOfWork.View.GetAllAsync(query);


      var viewDtos = _mapper.Map<IEnumerable<ViewDto>>(views).ToList();


      if (queryParameters.WithMovieInformation == true)
      {
        List<int> episodeIds = views.Select(c => c.EpisodeId).Distinct().ToList();
        var movies = await _movieService.GetEpsiodesInfor(new IdsRequestDto { Ids = episodeIds });

        // Tạo Dictionary để tối ưu việc tìm kiếm movie thông qua EpisodeId
        var movieDictionary = movies.ToDictionary(m => m.EpisodeId);

        foreach (var viewDto in viewDtos)
        {
          // Kiểm tra và lấy thông tin movie từ Dictionary
          if (movieDictionary.TryGetValue(viewDto.EpisodeId, out var episodeInfo))
          {
            viewDto.EpisodeInfor = episodeInfo;
          }
        }
      }


      // Gắn thông tin ánh xạ vào _response
      _response.Result = viewDtos;

      int totalItems = await _unitOfWork.View.CountAsync();
      var totalItemsPerPage = queryParameters.PageSize ?? totalItems;

      _response.Pagination = new PaginationDto
      {
        TotalItems = totalItems,
        TotalItemsPerPage = totalItemsPerPage,
        CurrentPage = queryParameters.PageNumber,
        TotalPages = (int)Math.Ceiling((double)totalItems / totalItemsPerPage)
      };

      return Ok(_response);
    }


    [HttpPost]
    [EnableRateLimiting("IpRateLimit")]
    public async Task<ActionResult<ResponseDto>> Post([FromQuery] AddViewRequestDto model)
    {

      var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
      //var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
      //           ?? HttpContext.Connection.RemoteIpAddress?.ToString();
      var cookieHandle = new DeviceCookieHandler();
      var ip = cookieHandle.Get(HttpContext);

      //if (ip == null && userId == null)
      //{
      //  throw new NotFoundException("Can't find IpAddress, please try again or login your account!");
      //}

      //View view = new View
      //{
      //  IpAddress = ip,
      //  UserId = userId,
      //  MovieId = model.MovieId,
      //  EpisodeId = model.EpisodeId,
      //  ViewDate = DateTime.UtcNow,
      //};


      //await _unitOfWork.View.AddAsync(view);
      //await _unitOfWork.SaveAsync();

      //var configApiKey = _configuration["ApiSettings:ApiKey"];

      //var message = await _movieService.IncreaseMovieView(new IncreaseMovieViewDto
      //{
      //  MovieId = model.MovieId,
      //  ApiKey = configApiKey
      //});

      _response.Result = ip;
      //_response.Message = message;

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
