namespace FlowerShop.Application
{
    public interface IFacadeService
    {
        IUserService UserService { get; }
        ICategoryService CategoryService { get; }
        IFlowerService FlowerService { get; }
        ICartService CartService { get; }
        IAddressService AddressService { get; }
        IOrderService OrderService { get; }
        ICartItemService CartItemService { get; }
        IPaymentService PaymentService { get; }
    }
}
