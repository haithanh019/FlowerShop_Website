using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FlowerShop.Infrastructure
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(FlowerShopDbContext context)
        {
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIDAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetByAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (!trackChanges)
                query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split([','], StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool trackChanges = false, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (!trackChanges)
                query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split([','], StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (!trackChanges)
                query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split([','], StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }

            return await query.Where(predicate).ToListAsync();
        }
        public IQueryable<T> GetQuery()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

    }
}
