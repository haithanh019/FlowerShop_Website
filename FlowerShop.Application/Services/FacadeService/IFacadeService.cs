namespace FlowerShop.Application
{
    public interface IFacadeService
    {
        IUserService UserService { get; }
        ICategoryService CategoryService { get; }
        IFlowerService FlowerService { get; }

    }
}
