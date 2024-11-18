using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public static class GenreFeatures
  {
    public static List<Expression<Func<Genre, bool>>> Filtering(GenreQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<Genre, bool>>>();

      var properties = typeof(GenreQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            case nameof(GenreQueryParameters.Name):
              filters.Add(m => m.Name.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(GenreQueryParameters.Slug):
              filters.Add(m => m.Slug.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(GenreQueryParameters.Status):
              filters.Add(m => m.Status == (bool)value);
              break;
          }
        }
      }

      return filters;
    }


    public static Func<IQueryable<Genre>, IOrderedQueryable<Genre>>? Sorting(GenreQueryParameters queryParameters)
    {
      Func<IQueryable<Genre>, IOrderedQueryable<Genre>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {
          "name" => isDescending
              ? (Func<IQueryable<Genre>, IOrderedQueryable<Genre>>)(q => q.OrderByDescending(m => m.Name))
              : q => q.OrderBy(m => m.Name),

          "slug" => isDescending
            ? (Func<IQueryable<Genre>, IOrderedQueryable<Genre>>)(q => q.OrderByDescending(m => m.Slug))
            : q => q.OrderBy(m => m.Slug),

          "status" => isDescending
              ? (Func<IQueryable<Genre>, IOrderedQueryable<Genre>>)(q => q.OrderByDescending(m => m.Status))
              : q => q.OrderBy(m => m.Status),

          "genreid" => isDescending
              ? (Func<IQueryable<Genre>, IOrderedQueryable<Genre>>)(q => q.OrderByDescending(m => m.GenreId))
              : q => q.OrderBy(m => m.GenreId),

          _ => q => q.OrderByDescending(m => m.GenreId)
        };
      }

      return orderByFunc;
    }




    public static QueryParameters<Genre> Build(GenreQueryParameters queryParameters)
    {
      return new QueryParameters<Genre>
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
