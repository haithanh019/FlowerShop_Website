
using FlowerShop.Infrastructure.Data;
using FlowerShop.Infrastructure.UnitOfWork;
using FlowerShop.Utility.Constants;
using FlowerShop.Utility.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<FlowerShopDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString(SystemConstants.ConnectionStringKey)));
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            var app = builder.Build();

            //Middlewares
            app.UseFlowerShopMiddleware();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
