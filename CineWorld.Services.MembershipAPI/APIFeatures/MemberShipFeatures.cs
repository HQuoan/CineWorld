using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Repositories;
using System.Linq.Expressions;

namespace CineWorld.Services.MembershipAPI.APIFeatures
{
  public static class MemberShipFeatures
  {
    public static List<Expression<Func<MemberShip, bool>>> Filtering(MemberShipQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<MemberShip, bool>>>();

      var properties = typeof(MemberShipQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            case nameof(MemberShipQueryParameters.UserEmail):
              filters.Add(m => m.UserEmail.ToLower().Contains(((string)value).ToLower()));
              break;
           
          }
        }
      }

      return filters;
    }


    public static Func<IQueryable<MemberShip>, IOrderedQueryable<MemberShip>>? Sorting(MemberShipQueryParameters queryParameters)
    {
      Func<IQueryable<MemberShip>, IOrderedQueryable<MemberShip>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {
          "membershipid" => isDescending
              ? (Func<IQueryable<MemberShip>, IOrderedQueryable<MemberShip>>)(q => q.OrderByDescending(m => m.MemberShipId))
              : q => q.OrderBy(m => m.MemberShipId),

          "useremail" => isDescending
            ? (Func<IQueryable<MemberShip>, IOrderedQueryable<MemberShip>>)(q => q.OrderByDescending(m => m.UserEmail))
            : q => q.OrderBy(m => m.UserEmail),

          "firstsubscriptiondate" => isDescending
              ? (Func<IQueryable<MemberShip>, IOrderedQueryable<MemberShip>>)(q => q.OrderByDescending(m => m.FirstSubscriptionDate))
              : q => q.OrderBy(m => m.FirstSubscriptionDate),

          "renewalstartdate" => isDescending
              ? (Func<IQueryable<MemberShip>, IOrderedQueryable<MemberShip>>)(q => q.OrderByDescending(m => m.RenewalStartDate))
              : q => q.OrderBy(m => m.RenewalStartDate),
          "lastupdateddate" => isDescending
               ? (Func<IQueryable<MemberShip>, IOrderedQueryable<MemberShip>>)(q => q.OrderByDescending(m => m.LastUpdatedDate))
               : q => q.OrderBy(m => m.LastUpdatedDate),
          "expirationdate" => isDescending
              ? (Func<IQueryable<MemberShip>, IOrderedQueryable<MemberShip>>)(q => q.OrderByDescending(m => m.ExpirationDate))
              : q => q.OrderBy(m => m.ExpirationDate),

          _ => q => q.OrderByDescending(m => m.MemberShipId)
        };
      }

      return orderByFunc;
    }




    public static QueryParameters<MemberShip> Build(MemberShipQueryParameters queryParameters)
    {
      return new QueryParameters<MemberShip>
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
