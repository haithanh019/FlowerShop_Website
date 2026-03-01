using FlowerShop.Domain;

namespace FlowerShop.Infrastructure
{
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(FlowerShopDbContext db)
            : base(db) { }
    }
}
