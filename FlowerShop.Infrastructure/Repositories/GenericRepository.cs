using FlowerShop.Infrastructure.Data;
using FlowerShop.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FlowerShop.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly FlowerShopDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(FlowerShopDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // GetById dùng FindAsync của EF Core (Mặc định là Tracking vì nó tìm trong Cache trước)
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool trackChanges = false)
        {
            // Nếu trackChanges = true => Dùng dbSet thường
            // Nếu trackChanges = false => Dùng AsNoTracking()
            return trackChanges
                ? await _dbSet.ToListAsync()
                : await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false)
        {
            // Bước 1: Khởi tạo query
            IQueryable<T> query = _dbSet;

            // Bước 2: Kiểm tra Tracking
            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            // Bước 3: Gắn điều kiện qua Where và thực thi
            return await query.Where(predicate).ToListAsync();
        }

        public async Task<T?> GetByAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false)
        {
            IQueryable<T> query = _dbSet;

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            // Attach để EF bắt đầu theo dõi nếu chưa theo dõi
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            // Khi xóa thì bắt buộc phải lấy object ra (Tracking) để xóa
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            Delete(entity);
            return true;
        }
    }
}
