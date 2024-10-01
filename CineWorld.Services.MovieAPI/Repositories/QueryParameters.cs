using System.Linq.Expressions;

namespace CineWorld.Services.MovieAPI.Repositories
{
  public class QueryParameters<T>
  {
    // Filtering
    public List<Expression<Func<T, bool>>>? Filters { get; set; } = new List<Expression<Func<T, bool>>>();

    // Including related entities
    public string? IncludeProperties { get; set; }

    // Sorting
    public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; }

    // Pagination
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }

    public QueryParameters() { }
  }

}
