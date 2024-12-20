using CineWorld.Services.ViewAPI.Models;
using CineWorld.Services.ViewAPI.Repositories;
using System.Linq.Expressions;

namespace CineWorld.Services.ViewAPI.APIFeatures
{
  public static class ViewStatFeatures
  {
    public static List<Expression<Func<View, bool>>> Filtering(ViewStatQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<View, bool>>>();

      var properties = typeof(ViewStatQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            
            case nameof(ViewStatQueryParameters.From):
              filters.Add(m => m.ViewDate >= (DateTime)value);
              break;
            case nameof(ViewStatQueryParameters.To):
              filters.Add(m => m.ViewDate <= (DateTime)value);
              break;
          }
        }
      }

      return filters;
    }


    public static Func<IQueryable<View>, IOrderedQueryable<View>>? Sorting(ViewStatQueryParameters queryParameters)
    {
      Func<IQueryable<View>, IOrderedQueryable<View>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {

          _ => q => q.OrderByDescending(m => m.ViewId)
        };
      }

      return orderByFunc;
    }

    public static QueryParameters<View> Build(ViewStatQueryParameters queryParameters)
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
