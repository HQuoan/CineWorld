using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories;
using System.Drawing.Printing;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public static class MovieFeatures
  {
    public static List<Expression<Func<Movie, bool>>> Filtering(MovieQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<Movie, bool>>>();

      var properties = typeof(MovieQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            case nameof(MovieQueryParameters.CategoryId):
              filters.Add(m => m.CategoryId == (int)value);
              break;
            case nameof(MovieQueryParameters.Category):
              filters.Add(m => m.Category.Name == (string)value);
              break;
            case nameof(MovieQueryParameters.CountryId):
              filters.Add(m => m.CountryId == (int)value);
              break;
            case nameof(MovieQueryParameters.Country):
              filters.Add(m => m.Country.Name == (string)value);
              break;
            case nameof(MovieQueryParameters.Name):
              filters.Add(m => m.Name.Contains((string)value) || (m.OriginName != null && m.OriginName.Contains((string)value)));
              break;
            case nameof(MovieQueryParameters.Year):
              filters.Add(m => m.Year == (int)value);
              break;
            case nameof(MovieQueryParameters.IsHot):
              filters.Add(m => m.IsHot == (bool)value);
              break;
            case nameof(MovieQueryParameters.Status):
              filters.Add(m => m.Status == (bool)value);
              break;
            case nameof(MovieQueryParameters.Genre):
              filters.Add(m => m.MovieGenres.Any(mg => mg.Genre.Name == (string)value));
              break;
          }
        }
      }

      return filters;
    }

    public static Func<IQueryable<Movie>, IOrderedQueryable<Movie>>? Sorting(MovieQueryParameters queryParameters)
    {
      Func<IQueryable<Movie>, IOrderedQueryable<Movie>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {
          "name" => isDescending ? (Func<IQueryable<Movie>, IOrderedQueryable<Movie>>)(q => q.OrderByDescending(m => m.Name))
                                  : q => q.OrderBy(m => m.Name),
          "movieid" => isDescending ? (Func<IQueryable<Movie>, IOrderedQueryable<Movie>>)(q => q.OrderByDescending(m => m.MovieId))
                                  : q => q.OrderBy(m => m.MovieId),
          "year" => isDescending ? (Func<IQueryable<Movie>, IOrderedQueryable<Movie>>)(q => q.OrderByDescending(m => m.Year))
                                  : q => q.OrderBy(m => m.Year),
          "category" => isDescending ? (Func<IQueryable<Movie>, IOrderedQueryable<Movie>>)(q => q.OrderByDescending(m => m.Category))
                                     : q => q.OrderBy(m => m.Category.Name),
          "country" => isDescending ? (Func<IQueryable<Movie>, IOrderedQueryable<Movie>>)(q => q.OrderByDescending(m => m.Country))
                                    : q => q.OrderBy(m => m.Country.Name),
          "isHot" => isDescending ? (Func<IQueryable<Movie>, IOrderedQueryable<Movie>>)(q => q.OrderByDescending(m => m.IsHot))
                                  : q => q.OrderBy(m => m.IsHot),
          "status" => isDescending ? (Func<IQueryable<Movie>, IOrderedQueryable<Movie>>)(q => q.OrderByDescending(m => m.Status))
                                   : q => q.OrderBy(m => m.Status),
          _ => q => q.OrderBy(m => m.CreatedDate) // Mặc định
        };
      }

      return orderByFunc;
    }



    public static QueryParameters<Movie> Build(MovieQueryParameters queryParameters)
    {
      return new QueryParameters<Movie>
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
