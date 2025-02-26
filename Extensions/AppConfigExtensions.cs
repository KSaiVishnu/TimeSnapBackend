using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Extensions
{
    public static class AppConfigExtensions
    {
        public static WebApplication ConfigureCORS(
            this WebApplication app,
            IConfiguration config)
        {
            app.UseCors(options =>
                    //options.AllowAnyOrigin()  // Allow all origins for now

            options.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader());
            app.UseCors("AllowAll");
            return app;
        }

        public static IServiceCollection AddAppConfig(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.Configure<AppSettings>(config.GetSection("AppSettings"));
            return services;
        }
    }
}







using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Backend.Models;
using Microsoft.AspNetCore.Builder;

namespace Backend.Extensions
{
    public static class AppConfigExtensions
    {
        // ✅ Configures CORS correctly and applies policy
        public static WebApplication ConfigureCORS(this WebApplication app)
        {
            app.UseCors("AllowAll");  // Apply the policy correctly
            return app;
        }

        // ✅ Registers application settings
        public static IServiceCollection AddAppConfig(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<AppSettings>(config.GetSection("AppSettings"));

            // ✅ Register CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }
    }
}
