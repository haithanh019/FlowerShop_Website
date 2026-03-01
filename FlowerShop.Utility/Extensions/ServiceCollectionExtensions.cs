using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlowerShop.Utility
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all utility services (Email, Cloudinary, etc.) and binds configurations.
        /// </summary>
        public static IServiceCollection AddFlowerShopUtility(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Bind Configuration Sections
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

            // 2. Register Services (Dependency Injection)
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();

            // 3. Add other helpers here in the future (e.g., Caching)

            return services;
        }
    }
}
