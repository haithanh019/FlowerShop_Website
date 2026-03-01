using FlowerShop.Domain;

namespace FlowerShop.Infrastructure
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(FlowerShopDbContext db)
            : base(db) { }
    }
}
