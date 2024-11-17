using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public static class SeriesFeatures
  {
    public static List<Expression<Func<Series, bool>>> Filtering(SeriesQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<Series, bool>>>();

      var properties = typeof(SeriesQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            case nameof(SeriesQueryParameters.Name):
              filters.Add(m => m.Name.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(SeriesQueryParameters.Slug):
              filters.Add(m => m.Slug.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(SeriesQueryParameters.Status):
              filters.Add(m => m.Status == (bool)value);
              break;
          }
        }
      }

      return filters;
    }


    public static Func<IQueryable<Series>, IOrderedQueryable<Series>>? Sorting(SeriesQueryParameters queryParameters)
    {
      Func<IQueryable<Series>, IOrderedQueryable<Series>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {
          "name" => isDescending
              ? (Func<IQueryable<Series>, IOrderedQueryable<Series>>)(q => q.OrderByDescending(m => m.Name))
              : q => q.OrderBy(m => m.Name),

          "slug" => isDescending
            ? (Func<IQueryable<Series>, IOrderedQueryable<Series>>)(q => q.OrderByDescending(m => m.Slug))
            : q => q.OrderBy(m => m.Slug),

          "status" => isDescending
              ? (Func<IQueryable<Series>, IOrderedQueryable<Series>>)(q => q.OrderByDescending(m => m.Status))
              : q => q.OrderBy(m => m.Status),

          "seriesid" => isDescending
              ? (Func<IQueryable<Series>, IOrderedQueryable<Series>>)(q => q.OrderByDescending(m => m.SeriesId))
              : q => q.OrderBy(m => m.SeriesId),

          _ => q => q.OrderByDescending(m => m.SeriesId)
        };
      }

      return orderByFunc;
    }




    public static QueryParameters<Series> Build(SeriesQueryParameters queryParameters)
    {
      return new QueryParameters<Series>
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
