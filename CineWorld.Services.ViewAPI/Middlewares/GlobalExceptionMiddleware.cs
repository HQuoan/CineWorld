using CineWorld.Services.ViewAPI.Exceptions;
using CineWorld.Services.ViewAPI.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;
using System.Text.Json;

namespace CineWorld.Services.ViewAPI.Attributes
{
  public class GlobalExceptionMiddleware
  {
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context); 
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Đã xảy ra ngoại lệ.");

        context.Response.ContentType = "application/json";

        var response = new ResponseDto
        {
          IsSuccess = false,
          Message = ex.Message 
        };

        switch (ex)
        {
          case NotFoundException _:
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            break;
          case DbUpdateException dbEx:
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.Message = dbEx.InnerException?.Message ?? "Đã xảy ra lỗi khi cập nhật cơ sở dữ liệu.";
            break;
          default:
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //response.Message = "Đã xảy ra lỗi không mong muốn.";
            response.Message = ex.Message;
            break;
        }

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
      }
    }
  }
}
