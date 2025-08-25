using BirthdayCakeAPI.Mapping;
using BusinessLogic.DTOs.Carts;
using BusinessLogic.DTOs.Carts.CartItems;
using BusinessLogic.DTOs.Categories;
using BusinessLogic.DTOs.CustomCakeOptions;
using BusinessLogic.DTOs.Orders;
using BusinessLogic.DTOs.Products;
using DataAccess.Data;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.FacadeService;
using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using System.Text;
using Ultitity.Email;
using Ultitity.Email.Interface;

namespace BirthdayCakeAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
            // Add services to the container.

            builder.Services.AddControllers().AddOData(opt =>
            {
                var modelBuilder = new ODataConventionModelBuilder();

                modelBuilder.EntitySet<CategoryDTO>("Categories");
                modelBuilder.EntitySet<ProductDTO>("Products");
                modelBuilder.EntitySet<CartItemDTO>("CartItems");
                modelBuilder.EntitySet<ShoppingCartDTO>("ShoppingCarts");
                modelBuilder.EntitySet<OrderDTO>("Orders");
                modelBuilder.EntitySet<CustomCakeOptionDTO>("CustomCakeOptions");


                opt.AddRouteComponents("odata", modelBuilder.GetEdmModel())
                    .Select().Filter().OrderBy().Expand().SetMaxTop(100).Count().SkipToken();
            });
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

            /// Register services for Application
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IFacadeService, FacadeService>();

            //builder.Services.AddScoped<ICategoryService, CategoryService>();
            //Services
            builder.Services.AddSingleton<IEmailQueue, EmailQueue>();
            builder.Services.AddSingleton<EmailSender>();
            builder.Services.AddHostedService<BackgroundEmailSender>();

            //Repositories
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



            //builder
            //    .Services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
