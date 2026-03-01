using FlowerShop.Domain;

namespace FlowerShop.Infrastructure
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(FlowerShopDbContext db)
            : base(db) { }
    }
}
