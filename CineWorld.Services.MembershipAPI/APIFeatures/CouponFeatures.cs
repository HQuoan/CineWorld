using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Repositories;
using System.Linq.Expressions;

namespace CineWorld.Services.MembershipAPI.APIFeatures
{
  public static class CouponFeatures
  {
    public static List<Expression<Func<Coupon, bool>>> Filtering(CouponQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<Coupon, bool>>>();

      var properties = typeof(CouponQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            case nameof(CouponQueryParameters.CouponCode):
              filters.Add(m => m.CouponCode.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(CouponQueryParameters.DiscountAmount):
              filters.Add(m => m.DiscountAmount == (decimal)value);
              break;
            case nameof(CouponQueryParameters.DurationInMonths):
              filters.Add(m => m.DurationInMonths == (int)value);
              break;
            case nameof(CouponQueryParameters.UsageCount):
              filters.Add(m => m.UsageCount == (int)value);
              break;
            case nameof(CouponQueryParameters.UsageLimit):
              filters.Add(m => m.UsageLimit == (int)value);
              break;
            case nameof(CouponQueryParameters.IsActive):
              filters.Add(m => m.IsActive == (bool)value);
              break;
          }
        }
      }

      return filters;
    }


    public static Func<IQueryable<Coupon>, IOrderedQueryable<Coupon>>? Sorting(CouponQueryParameters queryParameters)
    {
      Func<IQueryable<Coupon>, IOrderedQueryable<Coupon>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {
          "couponid" => isDescending
            ? (Func<IQueryable<Coupon>, IOrderedQueryable<Coupon>>)(q => q.OrderByDescending(m => m.CouponId))
            : q => q.OrderBy(m => m.CouponId),

          "couponcode" => isDescending
              ? (Func<IQueryable<Coupon>, IOrderedQueryable<Coupon>>)(q => q.OrderByDescending(m => m.CouponCode))
              : q => q.OrderBy(m => m.CouponCode),

          "discountamount" => isDescending
            ? (Func<IQueryable<Coupon>, IOrderedQueryable<Coupon>>)(q => q.OrderByDescending(m => m.DiscountAmount))
            : q => q.OrderBy(m => m.DiscountAmount),

          "durationinmonths" => isDescending
              ? (Func<IQueryable<Coupon>, IOrderedQueryable<Coupon>>)(q => q.OrderByDescending(m => m.DurationInMonths))
              : q => q.OrderBy(m => m.DurationInMonths),

          "usagelimit" => isDescending
              ? (Func<IQueryable<Coupon>, IOrderedQueryable<Coupon>>)(q => q.OrderByDescending(m => m.UsageLimit))
              : q => q.OrderBy(m => m.UsageLimit),

          "usagecount" => isDescending
              ? (Func<IQueryable<Coupon>, IOrderedQueryable<Coupon>>)(q => q.OrderByDescending(m => m.UsageCount))
              : q => q.OrderBy(m => m.UsageCount),

          "createddate" => isDescending
              ? (Func<IQueryable<Coupon>, IOrderedQueryable<Coupon>>)(q => q.OrderByDescending(m => m.CreatedDate))
              : q => q.OrderBy(m => m.CreatedDate),

          _ => q => q.OrderByDescending(m => m.CouponId)
        };
      }

      return orderByFunc;
    }




    public static QueryParameters<Coupon> Build(CouponQueryParameters queryParameters)
    {
      return new QueryParameters<Coupon>
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
