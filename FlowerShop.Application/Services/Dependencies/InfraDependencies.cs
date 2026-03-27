using Microsoft.Extensions.Configuration;
using PayOS;

namespace FlowerShop.Application
{
    public class InfraDependencies
    {
        public IConfiguration Configuration;
        public PayOSClient PayOSClientl;

        public InfraDependencies(IConfiguration configuration, PayOSClient payOSClient)
        {
            Configuration = configuration;
            PayOSClientl = payOSClient;
        }
    }
}
