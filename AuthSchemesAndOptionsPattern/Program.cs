
using AuthSchemesAndOptionsPattern.Data;
using AuthSchemesAndOptionsPattern.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>
            (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // configure the Identity 
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                options.Lockout.MaxFailedAccessAttempts = 5;

            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddRoles<IdentityRole>();

            builder.Services.AddScoped<JWTService>();
            // Adding Authentication  
            builder.Services.AddAuthentication(options =>
            {
                // cookies
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                // jwt

                //  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //  options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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

            //builder.Services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("CookiePolicy", policy =>
            //    {
            //        policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
            //        policy.RequireAuthenticatedUser();
            //    });
            //});

            //builder.Services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("JwtPolicy", policy =>
            //    {
            //        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
            //        policy.RequireAuthenticatedUser();
            //    });
            //});




            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

    //        builder.Services.AddEndpointsApiExplorer();
    //        builder.Services.AddSwaggerGen(options => {
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