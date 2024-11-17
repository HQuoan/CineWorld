using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Repositories;
using System.Linq.Expressions;

namespace CineWorld.Services.MembershipAPI.APIFeatures
{
  public static class PackageFeatures
  {
    public static List<Expression<Func<Package, bool>>> Filtering(PackageQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<Package, bool>>>();

      var properties = typeof(PackageQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            case nameof(PackageQueryParameters.Name):
              filters.Add(m => m.Name.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(PackageQueryParameters.Price):
              filters.Add(m => m.Price == (decimal)value);
              break;
            case nameof(PackageQueryParameters.TermInMonths):
              filters.Add(m => m.TermInMonths == (int)value);
              break;
            case nameof(PackageQueryParameters.Status):
              filters.Add(m => m.Status == (bool)value);
              break;
          }
        }
      }

      return filters;
    }


    public static Func<IQueryable<Package>, IOrderedQueryable<Package>>? Sorting(PackageQueryParameters queryParameters)
    {
      Func<IQueryable<Package>, IOrderedQueryable<Package>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {
          "packageid" => isDescending
              ? (Func<IQueryable<Package>, IOrderedQueryable<Package>>)(q => q.OrderByDescending(m => m.PackageId))
              : q => q.OrderBy(m => m.PackageId),

          "name" => isDescending
              ? (Func<IQueryable<Package>, IOrderedQueryable<Package>>)(q => q.OrderByDescending(m => m.Name))
              : q => q.OrderBy(m => m.Name),

          "price" => isDescending
            ? (Func<IQueryable<Package>, IOrderedQueryable<Package>>)(q => q.OrderByDescending(m => m.Price))
            : q => q.OrderBy(m => m.Price),

          "terminmonths" => isDescending
              ? (Func<IQueryable<Package>, IOrderedQueryable<Package>>)(q => q.OrderByDescending(m => m.TermInMonths))
              : q => q.OrderBy(m => m.TermInMonths),

          "createddate" => isDescending
              ? (Func<IQueryable<Package>, IOrderedQueryable<Package>>)(q => q.OrderByDescending(m => m.CreatedDate))
              : q => q.OrderBy(m => m.CreatedDate),

          _ => q => q.OrderByDescending(m => m.PackageId)
        };
      }

      return orderByFunc;
    }




    public static QueryParameters<Package> Build(PackageQueryParameters queryParameters)
    {
      return new QueryParameters<Package>
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
