using System.Linq.Expressions;

namespace BookShop.Application
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy = null,
            string includeProperties = "");

        Task<T> GetById(object id);
        Task Insert(T entity);
        Task Delete(object id);
        Task Delete(T entityToDelete);
        Task Update(T entityToUpdate);

        Task<List<T>> GetAll();
    }
}