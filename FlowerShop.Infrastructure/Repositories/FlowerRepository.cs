using FlowerShop.Domain;

namespace FlowerShop.Infrastructure
{
    public class FlowerRepository : GenericRepository<Flower>, IFlowerRepository
    {
        public FlowerRepository(FlowerShopDbContext db)
            : base(db) { }
    }
}
