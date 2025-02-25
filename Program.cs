
//using Backend.Controllers;
//using Backend.Extensions;
//using Backend.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//using Azure.Identity; // Added for Azure Authentication


//namespace Backend
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            builder.Services.AddDistributedMemoryCache();
//            builder.Services.AddMemoryCache();
//            builder.Services.AddHttpContextAccessor();
//            builder.Services.AddSession(options =>
//            {
//                options.IdleTimeout = TimeSpan.FromMinutes(5); // OTP expires in 5 mins
//                options.Cookie.HttpOnly = true;
//                options.Cookie.IsEssential = true;
//            });


//            builder.Services.AddControllers();

//            // ✅ Added Azure Authentication Credential Service
//            var azureCredential = new DefaultAzureCredential();
//            builder.Services.AddSingleton(azureCredential); // Inject DefaultAzureCredential

//            builder.Services.AddSwaggerExplorer()
//                //.InjectDbContext(builder.Configuration) // Uses Azure SQL Database now
//                .InjectDbContext(builder.Configuration)
//                .AddAppConfig(builder.Configuration)
//                .AddIdentityHandlersAndStores()                
//                .ConfigureIdentityOptions()
//                .AddIdentityAuth(builder.Configuration);

//            var app = builder.Build();

//            app.UseSession(); // Enable session
//            app.UseRouting();

//            app.ConfigureSwaggerExplorer()
//                .ConfigureCORS(builder.Configuration)
//                .AddIdentityAuthMiddlewares();

//            app.UseHttpsRedirection();

//            app.MapControllers();
//            app.MapGroup("/api")
//               .MapIdentityApi<AppUser>();
//            app.MapGroup("/api")
//               .MapIdentityUserEndpoints()
//               .MapAccountEndpoints()
//               .MapAuthorizationDemoEndpoints();

//            app.Run();
//        }
//    }
//}








//using Backend.Controllers;
//using Backend.Extensions;
//using Backend.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Azure.Identity; // Required for Azure Authentication
//using Microsoft.Data.SqlClient; // Required for SQL authentication handling

//namespace Backend
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            builder.Services.AddDistributedMemoryCache();
//            builder.Services.AddMemoryCache();
//            builder.Services.AddHttpContextAccessor();
//            builder.Services.AddSession(options =>
//            {
//                options.IdleTimeout = TimeSpan.FromMinutes(5); // OTP expires in 5 mins
//                options.Cookie.HttpOnly = true;
//                options.Cookie.IsEssential = true;
//            });

//            builder.Services.AddControllers();

//            // ✅ Inject DB Context (Uses Entra ID in Azure, Username/Password locally)
//            builder.Services.InjectDbContext(builder.Configuration);

//            builder.Services.AddSwaggerExplorer()
//                .AddAppConfig(builder.Configuration)
//                .AddIdentityHandlersAndStores()
//                .ConfigureIdentityOptions()
//                .AddIdentityAuth(builder.Configuration);

//            var app = builder.Build();

//            app.UseSession(); // Enable session
//            app.UseRouting();

//            app.ConfigureSwaggerExplorer()
//                .ConfigureCORS(builder.Configuration)
//                .AddIdentityAuthMiddlewares();

//            app.UseHttpsRedirection();

//            app.MapControllers();
//            app.MapGroup("/api")
//               .MapIdentityApi<AppUser>();
//            app.MapGroup("/api")
//               .MapIdentityUserEndpoints()
//               .MapAccountEndpoints()
//               .MapAuthorizationDemoEndpoints();

//            app.Run();
//        }
//    }
//}




using Backend.Controllers;
using Backend.Extensions;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5); // OTP expires in 5 mins
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllers();

            // ✅ Inject DB Context (Uses Azure Managed Identity for authentication)
            builder.Services.InjectDbContext(builder.Configuration);

            builder.Services.AddSwaggerExplorer()
                .AddAppConfig(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

            var app = builder.Build();

            app.UseSession(); // Enable session
            app.UseRouting();

            app.ConfigureSwaggerExplorer()
                .ConfigureCORS(builder.Configuration)
                .AddIdentityAuthMiddlewares();

            app.UseHttpsRedirection();

            app.MapControllers();
            app.MapGroup("/api")
               .MapIdentityApi<AppUser>();
            app.MapGroup("/api")
               .MapIdentityUserEndpoints()
               .MapAccountEndpoints()
               .MapAuthorizationDemoEndpoints();

            app.Run();
        }
    }
}
