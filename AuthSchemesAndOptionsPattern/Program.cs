
using AuthSchemesAndOptionsPattern.Data;
using AuthSchemesAndOptionsPattern.Helper;
using AuthSchemesAndOptionsPattern.Repository;
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
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(AppSettings.JWTKey));


            //builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("TopItems:ConnectionString"));
            //builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("TopItems:JWTKey"));

            //builder.Services.AddOptions<AppSettings>()
            //    .Bind(builder.Configuration.GetSection(AppSettings.ConnectionStrings))
            //    .ValidateDataAnnotations();

            // Add DbContext using IOptions
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

            

            // Adding Authentication  
            builder.Services.AddAuthentication(options =>
            {
                // cookies
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })

            .AddCookie()

            // Adding Jwt Bearer  
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWTKey:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWTKey:ValidIssuer"],
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTKey:Secret"]))
                };
            });



            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CookiePolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });

                options.AddPolicy("JwtPolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });



            // builder.Services.AddControllers();


            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

    //        builder.Services.AddEndpointsApiExplorer();
    //        builder.Services.AddSwaggerGen(options =>
    //        {
    //            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Permission Task -  API", Version = "v1" });
    //            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    //            {
    //                Name = "Authorization",
    //                In = ParameterLocation.Header,
    //                Type = SecuritySchemeType.ApiKey,
    //                Scheme = JwtBearerDefaults.AuthenticationScheme,
    //            });
    //            options.AddSecurityRequirement(new OpenApiSecurityRequirement
    //            {
    //    {
    //                    new OpenApiSecurityScheme
    //                    {
    //                        Reference =new OpenApiReference
    //                        {
    //                            Type = ReferenceType.SecurityScheme,
    //                            Id   = JwtBearerDefaults.AuthenticationScheme
    //                        },
    //                        Scheme = "Bearer",
    //                        Name   = JwtBearerDefaults.AuthenticationScheme,
    //                        In     = ParameterLocation.Header
    //                    },
    //                    new List<string>()
    //    }
    //});
    //        });

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