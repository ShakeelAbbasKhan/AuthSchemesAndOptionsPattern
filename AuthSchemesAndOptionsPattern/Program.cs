
using AuthSchemesAndOptionsPattern.Data;
using AuthSchemesAndOptionsPattern.Helper;
using AuthSchemesAndOptionsPattern.Repository;
using AuthSchemesAndOptionsPattern.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace AuthSchemesAndOptionsPattern
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Configure ConnectionStrings using IOptions

            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(AppSettings.ConnectionStrings));
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("JWTKey"));


            //builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("TopItems:ConnectionString"));
            //builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("TopItems:JWTKey"));

            //builder.Services.AddOptions<AppSettings>()
            //    .Bind(builder.Configuration.GetSection(AppSettings.ConnectionStrings))
            //    .ValidateDataAnnotations();

            // .ValidateOnStart();   validation eagerly


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    
            // configure the Identity 
                builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                options.Lockout.MaxFailedAccessAttempts = 5;

            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddRoles<IdentityRole>();




            builder.Services.AddScoped<JWTService>();
            builder.Services.AddSingleton<TestService>();

            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();


            builder.Services.ConfigureAuthentication(builder.Configuration);
            builder.Services.ConfigureAuthorization();


            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.ConfigureSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}