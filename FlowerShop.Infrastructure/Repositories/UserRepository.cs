using FlowerShop.Domain.Entities;
using FlowerShop.Infrastructure.Data;
using FlowerShop.Infrastructure.Repositories.Interfaces;

namespace FlowerShop.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(FlowerShopDbContext db)
            : base(db) { }
    }
}
