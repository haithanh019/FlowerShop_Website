
using FlowerShop.Application;
using FlowerShop.Infrastructure;
using FlowerShop.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Text;

namespace FlowerShop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<FlowerShopDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString(SystemConstants.ConnectionStringKey)));
            //Auto Mapper
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MapperProfile>();
            });
            //Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            //Facade Service
            builder.Services.AddScoped<IFacadeService, FacadeService>();
            //Utility Services
            builder.Services.AddFlowerShopUtility(builder.Configuration);

            //Orthers
            builder.Services.AddScoped<CoreDependencies>();
            builder.Services.AddScoped<InfraDependencies>();
            //OData
            static IEdmModel GetEdmModel()
            {
                var odataBuilder = new ODataConventionModelBuilder();
                odataBuilder.EntitySet<UserDTO>("Users");
                odataBuilder.EntitySet<CategoryDTO>("Categories");
                odataBuilder.EntitySet<FlowerDTO>("Flowers");
                odataBuilder.EntitySet<CartDTO>("Carts");
                odataBuilder.EntitySet<CartItemDTO>("CartItems");
                odataBuilder.EntitySet<AddressDTO>("Addresses");
                odataBuilder.EntitySet<OrderDTO>("Orders");
                return odataBuilder.GetEdmModel();
            }

            builder.Services.AddControllers()
            .AddOData(options => options
                .Select()
                .Filter()
                .OrderBy()
                .Expand()
                .Count()
                .SetMaxTop(100)
                .AddRouteComponents("Odata", GetEdmModel()));

            //JWT
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

            //Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                };
            });

            builder.Services.AddOpenApi();

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
