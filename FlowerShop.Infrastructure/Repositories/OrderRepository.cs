using FlowerShop.Domain;

namespace FlowerShop.Infrastructure
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(FlowerShopDbContext db)
            : base(db) { }
    }
}
