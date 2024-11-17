using CineWorld.Services.MovieAPI.Models;
using CineWorld.Services.MovieAPI.Repositories;
using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.APIFeatures
{
  public static class CategoryFeatures
  {
    public static List<Expression<Func<Category, bool>>> Filtering(CategoryQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<Category, bool>>>();

      var properties = typeof(CategoryQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            case nameof(CategoryQueryParameters.Name):
              filters.Add(m => m.Name.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(CategoryQueryParameters.Slug):
              filters.Add(m => m.Slug.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(CategoryQueryParameters.Status):
              filters.Add(m => m.Status == (bool)value);
              break;
          }
        }
      }

      return filters;
    }


    public static Func<IQueryable<Category>, IOrderedQueryable<Category>>? Sorting(CategoryQueryParameters queryParameters)
    {
      Func<IQueryable<Category>, IOrderedQueryable<Category>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {
          "name" => isDescending
              ? (Func<IQueryable<Category>, IOrderedQueryable<Category>>)(q => q.OrderByDescending(m => m.Name))
              : q => q.OrderBy(m => m.Name),

          "slug" => isDescending
            ? (Func<IQueryable<Category>, IOrderedQueryable<Category>>)(q => q.OrderByDescending(m => m.Slug))
            : q => q.OrderBy(m => m.Slug),

          "status" => isDescending
              ? (Func<IQueryable<Category>, IOrderedQueryable<Category>>)(q => q.OrderByDescending(m => m.Status))
              : q => q.OrderBy(m => m.Status),

          "categoryid" => isDescending
              ? (Func<IQueryable<Category>, IOrderedQueryable<Category>>)(q => q.OrderByDescending(m => m.CategoryId))
              : q => q.OrderBy(m => m.CategoryId),

          _ => q => q.OrderByDescending(m => m.CategoryId)
        };
      }

      return orderByFunc;
    }




    public static QueryParameters<Category> Build(CategoryQueryParameters queryParameters)
    {
      return new QueryParameters<Category>
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
