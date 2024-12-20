using AutoMapper;
using CineWorld.Services.MovieAPI.APIFeatures;
using CineWorld.Services.MovieAPI.Exceptions;
using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Models.Dtos;
using CineWorld.Services.MovieAPI.Repositories;
using CineWorld.Services.MovieAPI.Repositories.IRepositories;
using CineWorld.Services.MovieAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MovieAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ResponseDto _response;
        private readonly IUtil _util;

        public MovieAPIController(IUnitOfWork unitOfWork, IMapper mapper, IUtil util)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _response = new ResponseDto();
            _util = util;
        }

        /// <summary>
        /// Get all movies with optional filtering and pagination. Defaults to 25 movies per page, sorted by UpdateDate in descending order.
        /// </summary>
        /// <param name="queryParameters">The filtering and pagination parameters for the movie query.</param>
        /// <returns>A ResponseDto containing the list of movies and relevant metadata.</returns>
        [HttpGet]
        public async Task<ActionResult<ResponseDto>> Get([FromQuery] MovieQueryParameters? queryParameters)
        {
            if (!User.IsInRole(SD.AdminRole))
            {
                queryParameters.Status = true;
            }

            var query = MovieFeatures.Build(queryParameters);
            query.IncludeProperties = "Category,Country,Series,MovieGenres.Genre";

            IEnumerable<Movie> movies = await _unitOfWork.Movie.GetAllAsync(query);

            _response.Result = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);

            int totalItems = await _unitOfWork.Movie.CountAsync();
            _response.Pagination = new PaginationDto
            {
                TotalItems = totalItems,
                TotalItemsPerPage = queryParameters.PageSize,
                CurrentPage = queryParameters.PageNumber,
                TotalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize)
            };

            return Ok(_response);
        }

        /// <summary>
        /// Get a movie by its ID.
        /// </summary>
        /// <param name="id">The ID of the movie to retrieve.</param>
        /// <returns>A ResponseDto containing the movie details.</returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<ResponseDto>> Get(int id)
        {
            Movie movie;
            bool isAdmin = User.IsInRole(SD.AdminRole);
            if (isAdmin)
            {
                movie = await _unitOfWork.Movie.GetAsync(c => c.MovieId == id, includeProperties: "Category,Country,Series,MovieGenres.Genre");
            }
            else
            {
                movie = await _unitOfWork.Movie.GetAsync(c => c.MovieId == id && c.Status == true, includeProperties: "Category,Country,Series,MovieGenres.Genre");
            }

            if (movie == null)
            {
                throw new NotFoundException($"Movie with ID: {id} not found.");
            }

            var query = new QueryParameters<Episode>();
            query.Filters.Add(c => c.MovieId == movie.MovieId);

            if (!isAdmin)
            {
                query.Filters.Add(c => c.Status == true);
            }
            query.PageSize = null;

            movie.Episodes = await _unitOfWork.Episode.GetAllAsync(query);

            _response.Result = _mapper.Map<MovieDetailsDto>(movie);
            return Ok(_response);
        }

        /// <summary>
        /// Adds a new movie. Only accessible by users with the Admin role.
        /// </summary>
        /// <param name="movieDto">The movie data transfer object containing the details of the movie to be added.</param>
        /// <returns>A response containing the created movie data.</returns>
        [HttpPost]
        [Authorize(Roles = SD.AdminRole)]
        public async Task<ActionResult<ResponseDto>> Post([FromBody] MovieDto movieDto)
        {
            Movie movie = _mapper.Map<Movie>(movieDto);

            foreach (var genreId in movieDto.GenreIds)
            {
                if (genreId > 0)
                {
                    movie.MovieGenres.Add(new MovieGenre { GenreId = genreId });
                }
            }
            // Generate slug
            movie.Slug = SlugGenerator.GenerateSlug(movie.Name);

            try
            {
                await _unitOfWork.Movie.AddAsync(movie);
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException ex)
            {
                if (_util.IsUniqueConstraintViolation(ex))
                {
                    movie.Slug = SlugGenerator.CreateUniqueSlugAsync(movie.Name);
                    await _unitOfWork.Movie.AddAsync(movie);
                    await _unitOfWork.SaveAsync();
                }
            }

            _response.Result = _mapper.Map<MovieDto>(movie);
            return Created(string.Empty, _response);
        }

        /// <summary>
        /// Increases the view count of a movie by 1.
        /// </summary>
        /// <param name="id">The ID of the movie whose view count will be increased.</param>
        /// <returns>A response confirming the success of the operation.</returns>
        [HttpPost("IncreaseMovieView/{id}")]
        public async Task<ActionResult<ResponseDto>> IncreaseMovieView(int id)
        {
            Movie movie = await _unitOfWork.Movie.GetAsync(c => c.MovieId == id);

            if (movie == null)
            {
                throw new NotFoundException($"Movie with ID: {id} not found.");
            }

            movie.View++;
            await _unitOfWork.Movie.UpdateAsync(movie);
            await _unitOfWork.SaveAsync();

            _response.Message = $"Successfully added one view to the movie with ID: {id}.";
            return Ok(_response);
        }

        /// <summary>
        /// Updates the details of an existing movie. Only accessible by users with the Admin role.
        /// </summary>
        /// <param name="movieDto">The updated movie data transfer object.</param>
        /// <returns>A response containing the updated movie data.</returns>
        [HttpPut]
        [Authorize(Roles = SD.AdminRole)]
        public async Task<ActionResult<ResponseDto>> Put([FromBody] MovieDto movieDto)
        {
            var movieFromDb = await _unitOfWork.Movie.GetAsync(m => m.MovieId == movieDto.MovieId, includeProperties: "MovieGenres", tracked: true);
            if (movieFromDb == null)
            {
                throw new NotFoundException($"Movie with ID: {movieDto.MovieId} not found.");
            }

            // Xóa các thể loại hiện tại từ cơ sở dữ liệu
            movieFromDb.MovieGenres.Clear();
            await _unitOfWork.SaveAsync();

            // Generate slug
            if (movieFromDb.Name != movieDto.Name)
            {
                movieDto.Slug = SlugGenerator.GenerateSlug(movieDto.Name);
            }

            // Cập nhật các thuộc tính của movieFromDb từ movieDto
            _mapper.Map(movieDto, movieFromDb);

            foreach (var genreId in movieDto.GenreIds)
            {
                if (genreId > 0)
                {
                    movieFromDb.MovieGenres.Add(new MovieGenre { GenreId = genreId });
                }
            }

            movieFromDb.UpdatedDate = DateTime.UtcNow;

            try
            {
                await _unitOfWork.Movie.UpdateAsync(movieFromDb);
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException ex)
            {
                if (_util.IsUniqueConstraintViolation(ex))
                {
                    movieFromDb.Slug = SlugGenerator.CreateUniqueSlugAsync(movieFromDb.Name);
                    await _unitOfWork.Movie.UpdateAsync(movieFromDb);
                    await _unitOfWork.SaveAsync();
                }
            }

            _response.Result = _mapper.Map<MovieDto>(movieFromDb);
            return Ok(_response);
        }

        /// <summary>
        /// Deletes a movie by its ID. Only accessible by users with the Admin role.
        /// </summary>
        /// <param name="id">The ID of the movie to delete.</param>
        /// <returns>A response confirming the success of the delete operation.</returns>
        [HttpDelete]
        [Authorize(Roles = SD.AdminRole)]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var movie = await _unitOfWork.Movie.GetAsync(c => c.MovieId == id);
            if (movie == null)
            {
                throw new NotFoundException($"Movie with ID: {id} not found.");
            }

            await _unitOfWork.Movie.RemoveAsync(movie);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
        [HttpGet("GetMoviesById")]
        public async Task<ActionResult<ResponseDto>> GetMoviesById([FromQuery] List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "The list of IDs cannot be null or empty.";
                    return BadRequest(_response);
                }
                var filter = ids.ToArray();
                var movies = await _unitOfWork.Movie.GetAllAsync(new QueryParameters<Movie>
                {
                    Filters = new List<Expression<Func<Movie, bool>>>
            {
                movie => filter.Contains(movie.MovieId)
            }
                });
                if (movies == null || !movies.Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "No movies found for the provided IDs.";
                    return NotFound(_response);
                }
                var movieDtos = movies.Select(movie => new MovieResponseDto()
                {
                    MovieId = movie.MovieId,
                    MovieName = movie.Name,
                    MovieImageUrl = movie.ImageUrl
                }).ToList();

                _response.Result = movieDtos;
                return Ok(_response);
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(500, _response);
            }


        }
    }
}
