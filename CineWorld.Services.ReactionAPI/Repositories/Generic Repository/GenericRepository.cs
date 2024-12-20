
using CineWorld.Services.ReactionAPI.Data;
using System.Linq.Expressions;

namespace CineWorld.Services.ReactionAPI.Repositories.Generic_Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class

    {
        protected readonly AppDbContext _dbcontext;
        public GenericRepository(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public void Add(T entity)
        {

            _dbcontext.Set<T>().Add(entity);
        }
        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _dbcontext.Set<T>().Where(expression);
        }
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbcontext.Set<T>().FindAsync(id);
        }

        public void Update(T entity)
        {

            _dbcontext.Set<T>().Update(entity);

        }
        public void Delete(T entity)
        {

            _dbcontext.Set<T>().Remove(entity);
        }
        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbcontext.Set<T>().RemoveRange(entities);
        }
    }
}
