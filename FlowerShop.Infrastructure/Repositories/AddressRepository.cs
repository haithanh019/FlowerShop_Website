using FlowerShop.Domain;

namespace FlowerShop.Infrastructure
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(FlowerShopDbContext db)
            : base(db) { }
    }
}
