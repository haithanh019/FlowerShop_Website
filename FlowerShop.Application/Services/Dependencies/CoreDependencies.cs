using AutoMapper;
using FlowerShop.Infrastructure;

namespace FlowerShop.Application
{
    public class CoreDependencies
    {
        public IUnitOfWork UnitOfWork { get; set; }
        public IFacadeService FacadeService { get; set; }
        public IMapper Mapper { get; set; }
        public CoreDependencies(IUnitOfWork unitOfWork, IFacadeService facadeService, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            FacadeService = facadeService;
            Mapper = mapper;
        }
    }
}
