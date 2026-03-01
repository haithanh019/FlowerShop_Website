namespace FlowerShop.Infrastructure
{
    public interface IUnitOfWork
    {
        IAddressRepository AddressRepository { get; }
        ICartItemRepository CartItemRepository { get; }
        ICartRepository CartRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IFlowerImageRepository FlowerImageRepository { get; }
        IFlowerRepository FlowerRepository { get; }
        IOrderItemRepository OrderItemRepository { get; }
        IOrderRepository OrderRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IUserRepository UserRepository { get; }
        Task SaveAsync();
    }
}
