using CineWorld.Services.MembershipAPI.Models;
using CineWorld.Services.MembershipAPI.Repositories;
using System.Linq.Expressions;

namespace CineWorld.Services.MembershipAPI.APIFeatures
{
  public static class ReceiptFeatures
  {
    public static List<Expression<Func<Receipt, bool>>> Filtering(ReceiptQueryParameters queryParameters)
    {
      var filters = new List<Expression<Func<Receipt, bool>>>();

      var properties = typeof(ReceiptQueryParameters).GetProperties();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(queryParameters);
        if (value != null)
        {
          switch (prop.Name)
          {
            case nameof(ReceiptQueryParameters.UserId):
              filters.Add(m => m.UserId != null && m.UserId.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(ReceiptQueryParameters.Email):
              filters.Add(m => m.Email != null && m.Email.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(ReceiptQueryParameters.PackageId):
              filters.Add(m => m.PackageId == (int)value);
              break;
            case nameof(ReceiptQueryParameters.CouponCode):
              filters.Add(m => m.CouponCode != null && m.CouponCode.ToLower().Contains(((string)value).ToLower()));
              break;
            case nameof(ReceiptQueryParameters.PackagePrice):
              filters.Add(m => m.PackagePrice == (decimal)value);
              break;
            case nameof(ReceiptQueryParameters.TermInMonths):
              filters.Add(m => m.TermInMonths == (int)value);
              break;
            case nameof(ReceiptQueryParameters.CreatedDate):
              filters.Add(m => m.CreatedDate == (DateTime)value);
              break;
            case nameof(ReceiptQueryParameters.Status):
              filters.Add(m => m.Status != null && m.Status.ToLower() == ((string)value).ToLower());
              break;
          }
        }
      }

      return filters;
    }


    public static Func<IQueryable<Receipt>, IOrderedQueryable<Receipt>>? Sorting(ReceiptQueryParameters queryParameters)
    {
      Func<IQueryable<Receipt>, IOrderedQueryable<Receipt>>? orderByFunc = null;

      if (!string.IsNullOrEmpty(queryParameters.OrderBy))
      {
        var isDescending = queryParameters.OrderBy.StartsWith("-");
        var property = isDescending ? queryParameters.OrderBy.Substring(1) : queryParameters.OrderBy;

        orderByFunc = property.ToLower() switch
        {
          "receiptid" => isDescending
              ? (Func<IQueryable<Receipt>, IOrderedQueryable<Receipt>>)(q => q.OrderByDescending(m => m.ReceiptId))
              : q => q.OrderBy(m => m.ReceiptId),

          "userid" => isDescending
              ? (Func<IQueryable<Receipt>, IOrderedQueryable<Receipt>>)(q => q.OrderByDescending(m => m.UserId))
              : q => q.OrderBy(m => m.UserId),

          "email" => isDescending
              ? (Func<IQueryable<Receipt>, IOrderedQueryable<Receipt>>)(q => q.OrderByDescending(m => m.Email))
              : q => q.OrderBy(m => m.Email),

          "packageid" => isDescending
              ? (Func<IQueryable<Receipt>, IOrderedQueryable<Receipt>>)(q => q.OrderByDescending(m => m.PackageId))
              : q => q.OrderBy(m => m.PackageId),

          "couponcode" => isDescending
              ? (Func<IQueryable<Receipt>, IOrderedQueryable<Receipt>>)(q => q.OrderByDescending(m => m.CouponCode))
              : q => q.OrderBy(m => m.CouponCode),

          "packageprice" => isDescending
              ? (Func<IQueryable<Receipt>, IOrderedQueryable<Receipt>>)(q => q.OrderByDescending(m => m.PackagePrice))
              : q => q.OrderBy(m => m.PackagePrice),

          "terminmonths" => isDescending
              ? (Func<IQueryable<Receipt>, IOrderedQueryable<Receipt>>)(q => q.OrderByDescending(m => m.TermInMonths))
              : q => q.OrderBy(m => m.TermInMonths),

          "createddate" => isDescending
              ? (Func<IQueryable<Receipt>, IOrderedQueryable<Receipt>>)(q => q.OrderByDescending(m => m.CreatedDate))
              : q => q.OrderBy(m => m.CreatedDate),

          "status" => isDescending
              ? (Func<IQueryable<Receipt>, IOrderedQueryable<Receipt>>)(q => q.OrderByDescending(m => m.Status))
              : q => q.OrderBy(m => m.Status),

          "totalamount" => isDescending
              ? (Func<IQueryable<Receipt>, IOrderedQueryable<Receipt>>)(q => q.OrderByDescending(m => m.TotalAmount))
              : q => q.OrderBy(m => m.TotalAmount),

          _ => q => q.OrderByDescending(m => m.ReceiptId)
        };

      }

      return orderByFunc;
    }


    public static QueryParameters<Receipt> Build(ReceiptQueryParameters queryParameters)
    {
      return new QueryParameters<Receipt>
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
