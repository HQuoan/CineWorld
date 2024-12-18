using CineWorld.Services.ViewAPI.Models;
using CineWorld.Services.ViewAPI.Repositories;
using System.Linq.Expressions;

namespace CineWorld.Services.ViewAPI.APIFeatures
{
  public static class ViewFeatures
  {
    public static List<Expression<Func<View, bool>>> Filtering(ViewQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<View, bool>>>();

      var properties = typeof(ViewQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            case nameof(ViewQueryParameters.IpAddress):
              filters.Add(m => m.IpAddress == (string)value);
              break;
            case nameof(ViewQueryParameters.UserId):
              filters.Add(m => m.UserId == (string)value);
              break;
            case nameof(ViewQueryParameters.MovieId):
              filters.Add(m => m.MovieId == (int)value);
              break;
            case nameof(ViewQueryParameters.EpisodeId):
              filters.Add(m => m.EpisodeId == (int)value);
              break;
            case nameof(ViewQueryParameters.From):
              filters.Add(m => m.ViewDate >= (DateTime)value);
              break;
            case nameof(ViewQueryParameters.To):
              filters.Add(m => m.ViewDate <= (DateTime)value);
              break;
          }
        }
      }

      return filters;
    }


    public static Func<IQueryable<View>, IOrderedQueryable<View>>? Sorting(ViewQueryParameters queryParameters)
    {
      Func<IQueryable<View>, IOrderedQueryable<View>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {
          "ipaddress" => isDescending
              ? (Func<IQueryable<View>, IOrderedQueryable<View>>)(q => q.OrderByDescending(m => m.IpAddress))
              : q => q.OrderBy(m => m.IpAddress),

          "userid" => isDescending
              ? (Func<IQueryable<View>, IOrderedQueryable<View>>)(q => q.OrderByDescending(m => m.UserId))
              : q => q.OrderBy(m => m.UserId),

          "movieid" => isDescending
              ? (Func<IQueryable<View>, IOrderedQueryable<View>>)(q => q.OrderByDescending(m => m.MovieId))
              : q => q.OrderBy(m => m.MovieId),

          "episodeid" => isDescending
              ? (Func<IQueryable<View>, IOrderedQueryable<View>>)(q => q.OrderByDescending(m => m.EpisodeId))
              : q => q.OrderBy(m => m.EpisodeId),

          "viewdate" => isDescending
              ? (Func<IQueryable<View>, IOrderedQueryable<View>>)(q => q.OrderByDescending(m => m.ViewDate))
              : q => q.OrderBy(m => m.ViewDate),

          _ => q => q.OrderByDescending(m => m.ViewId)
        };
      }

      return orderByFunc;
    }

    public static QueryParameters<View> Build(ViewQueryParameters queryParameters)
    {
      return new QueryParameters<View>
      {
        Filters = Filtering(queryParameters),
        OrderBy = Sorting(queryParameters),
        IncludeProperties = queryParameters.IncludeProperties,
        PageNumber = queryParameters.PageNumber,
        PageSize = queryParameters.PageSize
      };
    }
  }
}
