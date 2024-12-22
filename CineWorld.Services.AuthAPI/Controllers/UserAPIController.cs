using AutoMapper;
using CineWorld.Services.AuthAPI.APIFeatures;
using CineWorld.Services.AuthAPI.Data;
using CineWorld.Services.AuthAPI.Exceptions;
using CineWorld.Services.AuthAPI.Models;
using CineWorld.Services.AuthAPI.Models.Dto;
using CineWorld.Services.AuthAPI.Services.IService;
using CineWorld.Services.AuthAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    //[Authorize]
    public class UserAPIController : ControllerBase
    {
        protected ResponseDto _response;
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IS3Service _s3Service;

        public UserAPIController(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IS3Service s3Service)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _response = new();
            _mapper = mapper;
            _s3Service = s3Service;

        }
        //[Authorize(Roles = SD.AdminRole)]

        /// <summary>
        /// Retrieves a list of all users. Only accessible by administrators.
        /// </summary>
        /// <returns>A list of users with their roles.</returns>
        /// <response code="200">Returns the list of users.</response>
        /// <response code="403">If the user is not authorized.</response>
        [HttpGet]
        [Authorize(Roles = SD.AdminRole)]
        public async Task<IActionResult> Get([FromQuery] UserQueryParameters queryParameters)
        {
            // Build query parameters
            var query = UserFeatures.Build(queryParameters);

            // Apply filters, sorting, and pagination (exclude Role)
            var queryableUsers = _db.ApplicationUsers.AsQueryable();

            if (query.Filters != null && query.Filters.Any())
            {
                foreach (var filter in query.Filters)
                {
                    queryableUsers = queryableUsers.Where(filter);
                }
            }

            if (query.OrderBy != null)
            {
                queryableUsers = query.OrderBy(queryableUsers);
            }

            // Retrieve total items before pagination
            var totalItemsBeforePagination = queryableUsers.Count();

            var users = await queryableUsers
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            // Retrieve roles for each user
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Role = string.Join(", ", roles);
            }

            // Filter by Role (in-memory)
            if (!string.IsNullOrEmpty(queryParameters.Role))
            {
                users = users
                    .Where(u => u.Role.ToLower().Contains(queryParameters.Role.ToLower()))
                    .ToList();
            }

            // Sort by Role (in-memory)
            if (!string.IsNullOrEmpty(queryParameters.OrderBy))
            {
                var isDescending = queryParameters.OrderBy.StartsWith("-");
                var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

                if (property.ToLower() == "role")
                {
                    users = isDescending
                        ? users.OrderByDescending(u => u.Role).ToList()
                        : users.OrderBy(u => u.Role).ToList();
                }
            }

            // Calculate pagination details
            var totalFilteredItems = users.Count; // Count after Role filter
            var paginatedUsers = users
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            // Map to DTO
            _response.Result = _mapper.Map<IEnumerable<UserDto>>(paginatedUsers);
            _response.Pagination = new PaginationDto
            {
                TotalItems = totalFilteredItems,
                TotalItemsPerPage = queryParameters.PageSize,
                CurrentPage = queryParameters.PageNumber,
                TotalPages = (int)Math.Ceiling((double)totalFilteredItems / queryParameters.PageSize)
            };

            return Ok(_response);
        }



        /// <summary>
        /// Retrieves detailed information about a specific user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user's details.</returns>
        /// <response code="200">Returns the user information.</response>
        /// <response code="403">If the user is not authorized to view the details.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {

            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!User.IsInRole(SD.AdminRole) && (userId != null && userId != id))
            {
                throw new UnauthorizedAccessException("You are not allowed to access data that does not belong to you.");
            }


            var user = await _userManager.FindByIdAsync(id);


            if (user == null)
            {
                throw new NotFoundException($"User with ID: {id} not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            user.Role = string.Join(", ", roles);

            _response.Result = _mapper.Map<UserDto>(user);

            return Ok(_response);
        }

        /// <summary>
        /// Checks if a user exists by ID.
        /// </summary>
        /// <param name="id">The ID of the user to check.</param>
        /// <returns>A boolean indicating whether the user exists.</returns>
        /// <response code="200">Returns true if the user exists, false otherwise.</response>
        [HttpGet("IsExistUser/{id}")]
        public async Task<bool> IsExistUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            return user != null;
        }
        [HttpGet("GetUserInformationById")]
        public async Task<ActionResult<ResponseDto>> GetUserInformationById([FromQuery] List<string> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "The list of IDs cannot be null or empty.";
                    return BadRequest(_response);
                }
                var userInformations = new List<UserCommentDTO>();
                foreach (var userId in ids)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        continue;
                    }
                    var userInfo = new UserCommentDTO
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Avatar = user.Avatar,
                    };
                    userInformations.Add(userInfo);
                }
                _response.Result = userInformations;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(500, _response);
            }
        }

        /// <summary>
        /// Retrieves user details by email.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>The user's details.</returns>
        /// <response code="200">Returns the user information.</response>
        /// <response code="403">If the user is not authorized to view the details.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpGet("GetByEmail/{email}")]
        [Authorize]
        public async Task<IActionResult> GetByEmail(string email)
        {
            string userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (!User.IsInRole(SD.AdminRole) && (userEmail != null && userEmail != email))
            {
                throw new UnauthorizedAccessException("You are not allowed to access data that does not belong to you.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new NotFoundException($"User with Email: {email} not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            user.Role = string.Join(", ", roles);

            _response.Result = _mapper.Map<UserDto>(user);

            return Ok(_response);
        }

        /// <summary>
        /// Updates user information. Accessible by administrators or the user themselves.
        /// </summary>
        /// <param name="userInformation">The updated user information.</param>
        /// <returns>The updated user information.</returns>
        /// <response code="200">Returns the updated user information.</response>
        /// <response code="403">If the user is not authorized to update the information.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpPut("UpdateInformation")]
        [Authorize]
        public async Task<IActionResult> UpdateInformation(UserInformation userInformation)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!User.IsInRole(SD.AdminRole) || (userId != null && userId != userInformation.Id))
            {
                throw new UnauthorizedAccessException("You are not allowed to access data that does not belong to you.");
            }

            var user = await _userManager.FindByIdAsync(userInformation.Id);
            if (user == null)
            {
                throw new NotFoundException($"User with ID: {userInformation.Id} not found.");
            }

            user.FullName = userInformation.FullName;
            user.Avatar = userInformation.Avatar;
            user.Gender = userInformation.Gender;
            user.DateOfBirth = userInformation.DateOfBirth;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            _response.Result = userInformation;
            return Ok(_response);
        }

        /// <summary>
        /// Deletes a user by ID. Only accessible by administrators.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        /// <response code="204">If the user is deleted successfully.</response>
        /// <response code="403">If the user is not authorized.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = SD.AdminRole)]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException($"User with ID: {id} not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
        [HttpPost("upload")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public async Task<IActionResult> UploadFile(IFormFile file, string pictureName, string? folder = null)
        {
            var fileKey = await _s3Service.UploadFileAsync(file.OpenReadStream(), file.FileName, pictureName, folder);
            var fileUrl = _s3Service.GetFileUrl(fileKey);

            return Ok(new { Url = fileUrl });
        }

        [HttpPost("updateAvatar")]
        [Authorize(Roles = $"{SD.AdminRole},{SD.CustomerRole}")]
        public async Task<IActionResult> UpdateAvatar(IFormFile file, string? folder = null)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userProfile = await _userManager.FindByIdAsync(userId);
            if (userProfile == null)
            {
                return NotFound("User profile not found.");
            }
            var key = await _s3Service.UploadFileAsync(file.OpenReadStream(), file.FileName, userId, folder);
            userProfile.Avatar = _s3Service.GetFileUrl(key);
            var result = await _userManager.UpdateAsync(userProfile);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            _response.Result = userProfile.Avatar;
            return Ok(_response);
        }

    }
}