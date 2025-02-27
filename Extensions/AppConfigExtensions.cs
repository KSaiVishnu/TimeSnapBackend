using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Extensions
{
    //public static class AppConfigExtensions
    //{
    //    public static WebApplication ConfigureCORS(
    //        this WebApplication app,
    //        IConfiguration config)
    //    {
    //        app.UseCors(options =>
    //                //options.AllowAnyOrigin()  // Allow all origins for now

    //        options.WithOrigins("http://localhost:4200")
    //        .AllowAnyMethod()
    //        .AllowAnyHeader());
    //        app.UseCors("AllowAll");
    //        return app;
    //    }

    //    public static IServiceCollection AddAppConfig(
    //        this IServiceCollection services,
    //        IConfiguration config)
    //    {
    //        services.Configure<AppSettings>(config.GetSection("AppSettings"));
    //        return services;
    //    }
    //}


    public static class AppConfigExtensions
    {
        public static WebApplication ConfigureCORS(
            this WebApplication app,
            IConfiguration config)
        {
            app.UseCors("AllowAll"); // Use the named policy correctly
            return app;
        }

        public static IServiceCollection AddAppConfig(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.Configure<AppSettings>(config.GetSection("AppSettings"));

            // ✅ Define the named CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder.WithOrigins("http://localhost:4200") // Change as needed
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials());
            });

            return services;
        }
    }



}
