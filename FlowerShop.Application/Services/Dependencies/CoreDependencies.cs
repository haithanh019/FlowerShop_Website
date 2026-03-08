using AutoMapper;
using FlowerShop.Infrastructure;

namespace FlowerShop.Application
{
    public class CoreDependencies
    {
        public IUnitOfWork UnitOfWork { get; set; }
        public IMapper Mapper { get; set; }
        public CoreDependencies(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
    }
}
