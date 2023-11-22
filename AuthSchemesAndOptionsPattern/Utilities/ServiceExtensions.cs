using AuthSchemesAndOptionsPattern.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AuthSchemesAndOptionsPattern.Utilities
{
    public static class ServiceExtensions
    {
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JWT");
            var jwtConfiguration = new AppSettings();
            configuration.Bind(jwtConfiguration.JWTKey, jwtConfiguration);


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "MultiAuthSchemes";
                options.DefaultChallengeScheme = "MultiAuthSchemes";
                options.DefaultScheme = "MultiAuthSchemes";
            })

             .AddCookie(options =>
                {
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        }
                    };
                })

            // Adding Jwt Bearer  
            .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = configuration["JWTKey:ValidAudience"],
                        ValidIssuer = configuration["JWTKey:ValidIssuer"],
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTKey:Secret"]))
                    };
                })

            .AddPolicyScheme("MultiAuthSchemes", JwtBearerDefaults.AuthenticationScheme, options =>
                 {
                     options.ForwardDefaultSelector = context =>
                     {
                         string authorization = context.Request.Headers[HeaderNames.Authorization];
                         if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                         {
                             var token = authorization.Substring("Bearer ".Length).Trim();
                             var jwtHandler = new JwtSecurityTokenHandler();
                         
                             bool IsValidJWT = (jwtHandler.CanReadToken(token) && jwtHandler.ReadJwtToken(token).Issuer.Equals(jwtConfiguration.ValidIssuer));
                             if (IsValidJWT)
                             {
                                 return JwtBearerDefaults.AuthenticationScheme;
                             }
                         }
                         return CookieAuthenticationDefaults.AuthenticationScheme;
                     };
                 });
        }
        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
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

                options.AddPolicy("CookieOrJwtPolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });
        }

        public static void ConfigureSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Permission Task -  API", Version = "v1" });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference =new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = JwtBearerDefaults.AuthenticationScheme
                            },
                            Scheme = "Bearer",
                            Name   = JwtBearerDefaults.AuthenticationScheme,
                            In     = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }

    }
}
