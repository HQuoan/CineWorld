using CineWorld.Services.MovieAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using CineWorld.Services.MovieAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CineWorld.Services.CouponAPI.Attributes
{
  public class ExceptionHandlingAttribute : ActionFilterAttribute
  {
    //private readonly ILogger<ExceptionHandlingAttribute> _logger;

    //public ExceptionHandlingAttribute(ILogger<ExceptionHandlingAttribute> logger)
    //{
    //  _logger = logger;
    //}

    public override void OnActionExecuted(ActionExecutedContext context)
    {
      if (context.Exception != null)
      {
        var exceptionType = context.Exception.GetType();
        var response = new ResponseDto
        {
          IsSuccess = false,
          //Message = "An error occurred while processing your request.",
          Message = context.Exception.Message,
        };

        // Log the exception for debugging purposes
        //_logger.LogError(context.Exception, "An error occurred during the request.");

        // Handle specific exceptions
       if (exceptionType == typeof(NotFoundException)) // Handle NotFoundException
        {
          response.Message = context.Exception.Message;
          context.Result = new JsonResult(response)
          {
            StatusCode = StatusCodes.Status404NotFound,
          };
        }
        else if (context.Exception is DbUpdateException dbUpdateException) // Handle DbUpdateException
        {
          var innerException = dbUpdateException.InnerException;
          if (innerException != null)
          {
            response.Message = $"Database error: {innerException.Message}";
          }
          else
          {
            response.Message = "An error occurred while updating the database.";
          }

          context.Result = new JsonResult(response)
          {
            StatusCode = StatusCodes.Status400BadRequest, // Có thể là 400 hoặc một mã khác tùy thuộc vào lỗi cụ thể
          };
        }
        else
        {
          context.Result = new JsonResult(response)
          {
            StatusCode = StatusCodes.Status500InternalServerError,
          };
        }

        context.ExceptionHandled = true;
      }
    }
  }
}
