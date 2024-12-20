using CineWorld.Services.AuthAPI.Models;
using System.Linq.Expressions;

namespace CineWorld.Services.AuthAPI.APIFeatures
{
  public static class UserFeatures
  {
    public static List<Expression<Func<ApplicationUser, bool>>> Filtering(UserQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<ApplicationUser, bool>>>();

      var properties = typeof(UserQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            case nameof(UserQueryParameters.Email):
              filters.Add(m => m.Email.ToLower().Contains(((string)value).ToLower()));
              break;

            case nameof(UserQueryParameters.FullName):
              filters.Add(m => m.FullName.ToLower().Contains(((string)value).ToLower()));
              break;

            case nameof(UserQueryParameters.Gender):
              filters.Add(m => m.Gender.ToLower().Contains(((string)value).ToLower()));
              break;

            case nameof(UserQueryParameters.DateOfBirth):
              filters.Add(m => m.DateOfBirth == (DateTime)value);
              break;

          }
        }
      }

      return filters;
    }


    public static Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>? Sorting(UserQueryParameters queryParameters)
    {
      Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {
          "fullname" => isDescending
              ? (Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>)(q => q.OrderByDescending(m => m.FullName))
              : q => q.OrderBy(m => m.FullName),

          "email" => isDescending
              ? (Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>)(q => q.OrderByDescending(m => m.Email))
              : q => q.OrderBy(m => m.Email),


          _ => q => q.OrderByDescending(m => m.Id) // Mặc định sắp xếp theo UpdatedDate giảm dần
        };
      }

      return orderByFunc;
    }




    public static QueryParameters<ApplicationUser> Build(UserQueryParameters queryParameters)
    {
      return new QueryParameters<ApplicationUser>
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
