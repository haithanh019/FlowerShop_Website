using System.Linq.Expressions;

namespace FlowerShop.Infrastructure.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIDAsync(Guid id);
        Task<T?> GetByAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false);
        Task<IEnumerable<T>> GetAllAsync(bool trackChanges = false);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}
