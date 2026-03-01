using FlowerShop.Domain;

namespace FlowerShop.Infrastructure
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(FlowerShopDbContext db)
            : base(db) { }
    }
}
