using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CineWorld.Services.ViewAPI.Repositories
{
  public class QueryParameters<T>
  {
    // Filtering
    public List<Expression<Func<T, bool>>>? Filters { get; set; } = new List<Expression<Func<T, bool>>>();

    // Including related entities
    public string? IncludeProperties { get; set; }

    // Sorting
    public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; } = query =>
    {
      var primaryKey = typeof(T).GetProperties()
                                .FirstOrDefault(p => p.Name.EndsWith("Id"))?.Name;

      return (IOrderedQueryable<T>)(primaryKey != null
          ? query.OrderByDescending(e => EF.Property<object>(e, primaryKey))
          : query);
    };


    // Pagination
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 25;

  }

}
