using FlowerShop.Utility;
using Microsoft.Extensions.Options;

namespace FlowerShop.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FlowerShopDbContext _db;

        public IAddressRepository AddressRepository { get; private set; }
        public ICartItemRepository CartItemRepository { get; private set; }
        public ICartRepository CartRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }
        public IFlowerImageRepository FlowerImageRepository { get; private set; }
        public IFlowerRepository FlowerRepository { get; private set; }
        public IOrderItemRepository OrderItemRepository { get; private set; }
        public IOrderRepository OrderRepository { get; private set; }
        public IPaymentRepository PaymentRepository { get; private set; }
        public IUserRepository UserRepository { get; private set; }
        public UnitOfWork(FlowerShopDbContext db, IOptions<CloudinarySettings> cloudinaryOptions)
        {
            _db = db;
            AddressRepository = new AddressRepository(_db);
            CartItemRepository = new CartItemRepository(_db);
            CartRepository = new CartRepository(_db);
            CategoryRepository = new CategoryRepository(_db);
            FlowerImageRepository = new FlowerImageRepository(_db, cloudinaryOptions);
            FlowerRepository = new FlowerRepository(_db);
            OrderItemRepository = new OrderItemRepository(_db);
            OrderRepository = new OrderRepository(_db);
            PaymentRepository = new PaymentRepository(_db);
            UserRepository = new UserRepository(_db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
