namespace FlowerShop.Application
{
    public class FacadeService : IFacadeService
    {
        public IUserService UserService { get; }
        public ICategoryService CategoryService { get; }
        public IFlowerService FlowerService { get; }
        public ICartService CartService { get; }
        public FacadeService(CoreDependencies coreDependencies, InfraDependencies infraDependencies)
        {
            UserService = new UserService(coreDependencies.UnitOfWork, coreDependencies.Mapper, infraDependencies.Configuration);
            CategoryService = new CategoryService(coreDependencies.UnitOfWork, coreDependencies.Mapper);
            FlowerService = new FlowerService(coreDependencies.UnitOfWork, coreDependencies.Mapper);
            CartService = new CartService(coreDependencies.UnitOfWork, coreDependencies.Mapper);
        }
    }
}
