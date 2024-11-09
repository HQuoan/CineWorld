using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public static class CountryFeatures
  {
    public static List<Expression<Func<Country, bool>>> Filtering(CountryQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<Country, bool>>>();

      var properties = typeof(CountryQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            case nameof(CountryQueryParameters.Name):
              filters.Add(m => m.Name.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(CountryQueryParameters.Slug):
              filters.Add(m => m.Slug.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(CountryQueryParameters.Status):
              filters.Add(m => m.Status == (bool)value);
              break;
          }
        }
      }

      return filters;
    }


    public static Func<IQueryable<Country>, IOrderedQueryable<Country>>? Sorting(CountryQueryParameters queryParameters)
    {
      Func<IQueryable<Country>, IOrderedQueryable<Country>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {
          "name" => isDescending
              ? (Func<IQueryable<Country>, IOrderedQueryable<Country>>)(q => q.OrderByDescending(m => m.Name))
              : q => q.OrderBy(m => m.Name),

          "slug" => isDescending
            ? (Func<IQueryable<Country>, IOrderedQueryable<Country>>)(q => q.OrderByDescending(m => m.Slug))
            : q => q.OrderBy(m => m.Slug),

          "status" => isDescending
              ? (Func<IQueryable<Country>, IOrderedQueryable<Country>>)(q => q.OrderByDescending(m => m.Status))
              : q => q.OrderBy(m => m.Status),

          "countryid" => isDescending
              ? (Func<IQueryable<Country>, IOrderedQueryable<Country>>)(q => q.OrderByDescending(m => m.CountryId))
              : q => q.OrderBy(m => m.CountryId),

          _ => q => q.OrderByDescending(m => m.CountryId)
        };
      }

      return orderByFunc;
    }




    public static QueryParameters<Country> Build(CountryQueryParameters queryParameters)
    {
      return new QueryParameters<Country>
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
