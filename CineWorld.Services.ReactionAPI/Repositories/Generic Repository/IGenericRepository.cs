using System.Linq.Expressions;

namespace CineWorld.Services.ReactionAPI.Repositories.Generic_Repository
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void Update(T entity);
        void Delete(T enity);
        void DeleteRange(IEnumerable<T> entities);
        Task<T> GetByIdAsync(int id);
        
        
    }
}
