using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public static class EpisodeFeatures
  {
    public static List<Expression<Func<Episode, bool>>> Filtering(EpisodeQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<Episode, bool>>>();

      var properties = typeof(EpisodeQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            case nameof(EpisodeQueryParameters.Name):
              filters.Add(m => m.Name.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(EpisodeQueryParameters.MovieId):
              filters.Add(m => m.MovieId == (int)value);
              break;
            case nameof(EpisodeQueryParameters.EpisodeNumber):
              filters.Add(m => m.EpisodeNumber == (int)value);
              break;
            case nameof(EpisodeQueryParameters.Status):
              filters.Add(m => m.Status == (bool)value);
              break;
            case nameof(EpisodeQueryParameters.IsFree):
              filters.Add(m => m.IsFree == (bool)value);
              break;
          }
        }
      }

      return filters;
    }


    public static Func<IQueryable<Episode>, IOrderedQueryable<Episode>>? Sorting(EpisodeQueryParameters queryParameters)
    {
      Func<IQueryable<Episode>, IOrderedQueryable<Episode>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {
          "name" => isDescending
              ? (Func<IQueryable<Episode>, IOrderedQueryable<Episode>>)(q => q.OrderByDescending(m => m.Name))
              : q => q.OrderBy(m => m.Name),

          "episodenumber" => isDescending
          ? (Func<IQueryable<Episode>, IOrderedQueryable<Episode>>)(q => q.OrderByDescending(m => m.EpisodeNumber))
          : q => q.OrderBy(m => m.EpisodeNumber),

          "status" => isDescending
              ? (Func<IQueryable<Episode>, IOrderedQueryable<Episode>>)(q => q.OrderByDescending(m => m.Status))
              : q => q.OrderBy(m => m.Status),

          "episodeid" => isDescending
              ? (Func<IQueryable<Episode>, IOrderedQueryable<Episode>>)(q => q.OrderByDescending(m => m.EpisodeId))
              : q => q.OrderBy(m => m.EpisodeId),

          _ => q => q.OrderByDescending(m => m.EpisodeId)
        };
      }

      return orderByFunc;
    }




    public static QueryParameters<Episode> Build(EpisodeQueryParameters queryParameters)
    {
      return new QueryParameters<Episode>
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
