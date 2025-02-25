using Backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using System.Threading.Tasks;
using System.Linq;

namespace Backend.Extensions
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using System.Linq;

    public class CustomEmailValidator<TUser> : IUserValidator<TUser> where TUser : IdentityUser
    {
        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            if (!user.Email.EndsWith(".ac.in"))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidEmailDomain",
                    Description = "Only email addresses ending with .ac.in are allowed."
                }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }

    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityHandlersAndStores(this IServiceCollection services)
        {
            services.AddIdentityApiEndpoints<AppUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>();
            return services;
        }

        public static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
                //options.User.RequireUniqueEmail = false;
            });
            services.AddScoped<IUserValidator<AppUser>, CustomEmailValidator<AppUser>>();
            return services;
        }

        //Auth = Authentication + Authorization
        public static IServiceCollection AddIdentityAuth(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(y =>
            {
                y.SaveToken = false;
                y.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                config["AppSettings:JWTSecret"]!)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                  .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                  .RequireAuthenticatedUser()
                  .Build();
            });
                return services; 
        }

        public static WebApplication AddIdentityAuthMiddlewares(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}

