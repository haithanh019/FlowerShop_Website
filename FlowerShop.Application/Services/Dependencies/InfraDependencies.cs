using Microsoft.Extensions.Configuration;

namespace FlowerShop.Application
{
    public class InfraDependencies
    {
        public IConfiguration Configuration;

        public InfraDependencies(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
