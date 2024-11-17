using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public static class ServerFeatures
  {
    public static List<Expression<Func<Server, bool>>> Filtering(ServerQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<Server, bool>>>();

      var properties = typeof(ServerQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            case nameof(ServerQueryParameters.Name):
              filters.Add(m => m.Name.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(ServerQueryParameters.EpisodeId):
              filters.Add(m => m.EpisodeId == (int)value);
              break;
          }
        }
      }

      return filters;
    }


    public static Func<IQueryable<Server>, IOrderedQueryable<Server>>? Sorting(ServerQueryParameters queryParameters)
    {
      Func<IQueryable<Server>, IOrderedQueryable<Server>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {
          "name" => isDescending
              ? (Func<IQueryable<Server>, IOrderedQueryable<Server>>)(q => q.OrderByDescending(m => m.Name))
              : q => q.OrderBy(m => m.Name),

          "episodeid" => isDescending
              ? (Func<IQueryable<Server>, IOrderedQueryable<Server>>)(q => q.OrderByDescending(m => m.EpisodeId))
              : q => q.OrderBy(m => m.EpisodeId),

          "link" => isDescending
              ? (Func<IQueryable<Server>, IOrderedQueryable<Server>>)(q => q.OrderByDescending(m => m.Link))
              : q => q.OrderBy(m => m.Link),

          "categoryid" => isDescending
              ? (Func<IQueryable<Server>, IOrderedQueryable<Server>>)(q => q.OrderByDescending(m => m.ServerId))
              : q => q.OrderBy(m => m.ServerId),

          _ => q => q.OrderByDescending(m => m.ServerId)
        };
      }

      return orderByFunc;
    }


    public static QueryParameters<Server> Build(ServerQueryParameters queryParameters)
    {
      return new QueryParameters<Server>
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
