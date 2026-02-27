using System.Linq.Expressions;

namespace FlowerShop.Infrastructure.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // (Read)
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> GetByAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false);
        Task<IEnumerable<T>> GetAllAsync(bool trackChanges = false);

        // (Filter)
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false);
        // (Create)
        Task AddAsync(T entity);

        // (Update) - 
        void Update(T entity);

        // Xóa (Delete)
        void Delete(T entity);
        Task<bool> DeleteAsync(Guid id);
    }
}
