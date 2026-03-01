using FlowerShop.Domain;

namespace FlowerShop.Infrastructure
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(FlowerShopDbContext db)
            : base(db) { }
    }
}
