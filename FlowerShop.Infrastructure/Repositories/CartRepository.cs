using FlowerShop.Domain.Entities;
using FlowerShop.Infrastructure.Data;
using FlowerShop.Infrastructure.Repositories.Interfaces;

namespace FlowerShop.Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(FlowerShopDbContext db)
            : base(db) { }
    }
}
