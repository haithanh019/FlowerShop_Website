using Microsoft.AspNetCore.Builder;

namespace FlowerShop.Utility
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds global exception handling and other utility middlewares.
        /// </summary>
        public static IApplicationBuilder UseFlowerShopMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // Future middlewares can go here (e.g., RequestLogging)

            return app;
        }
    }
}
