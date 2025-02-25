//using Backend.Models;
//using Microsoft.EntityFrameworkCore;

//namespace Backend.Extensions
//{
//    public static class EFCoreExtensions
//    {
//        public static IServiceCollection InjectDbContext(
//            this IServiceCollection services,
//            IConfiguration config)
//        {
//            services.AddDbContext<AppDbContext>(options =>
//                     options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
//            return services;
//        }
//    }
//}


//using Backend.Models;
//using Microsoft.EntityFrameworkCore;
//using Azure.Identity; // ✅ Added Azure Authentication
//using Azure.Core;
//using Microsoft.Data.SqlClient;
//using System.Data.Common;
//using Microsoft.EntityFrameworkCore.Diagnostics; // ✅ Import the required namespace

//namespace Backend.Extensions
//{
//    public static class EFCoreExtensions
//    {
//        public static IServiceCollection InjectDbContext(
//            this IServiceCollection services,
//            IConfiguration config)
//        {
//            var connectionString = config.GetConnectionString("DefaultConnection"); // ✅ Get Azure SQL connection string
//            var azureCredential = new DefaultAzureCredential(); // ✅ Added Azure AD Authentication

//            services.AddDbContext<AppDbContext>(options =>
//            {
//                options.UseSqlServer(connectionString, opt => opt.EnableRetryOnFailure()); // ✅ Retry policy for Azure SQL
//                options.AddInterceptors(new AzureAdAuthenticationInterceptor(azureCredential)); // ✅ Uses Azure AD authentication
//            });

//            return services;
//        }
//    }

//    // ✅ Custom Interceptor to handle Azure AD authentication
//    public class AzureAdAuthenticationInterceptor : DbConnectionInterceptor
//    {
//        private readonly TokenCredential _tokenCredential;

//        public AzureAdAuthenticationInterceptor(TokenCredential tokenCredential)
//        {
//            _tokenCredential = tokenCredential;
//        }

//        // ✅ Correct method: `ConnectionOpenedAsync` instead of `ConnectionOpeningAsync`
//        public override async Task ConnectionOpenedAsync(
//            DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
//        {
//            if (connection is SqlConnection sqlConnection)
//            {
//                var token = await _tokenCredential.GetTokenAsync(
//                    new TokenRequestContext(new[] { "https://database.windows.net/.default" }), cancellationToken); // ✅ Requests access token for Azure SQL
//                sqlConnection.AccessToken = token.Token; // ✅ Assigns token to SQL Connection
//            }

//            await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
//        }
//    }
//}



//using Backend.Models;
//using Microsoft.EntityFrameworkCore;
//using Azure.Identity;  // Required for Azure authentication
//using Microsoft.Data.SqlClient;  // Required for SQL connection handling

//namespace Backend.Extensions
//{
//    public static class EFCoreExtensions
//    {
//        public static IServiceCollection InjectDbContext(
//            this IServiceCollection services,
//            IConfiguration config)
//        {
//            var connectionString = config.GetConnectionString("DefaultConnection");

//            // Check if connection string is configured for Entra ID authentication
//            if (connectionString.Contains("Authentication=Active Directory Default"))
//            {
//                var azureCredential = new DefaultAzureCredential(); // Uses Managed Identity or Service Principal

//                services.AddDbContext<AppDbContext>(options =>
//                    options.UseSqlServer(new SqlConnection(connectionString)
//                    {
//                        AccessToken = azureCredential.GetToken(
//                            new Azure.Core.TokenRequestContext(new[] { "https://database.windows.net/.default" })
//                        ).Token
//                    }));
//            }
//            else
//            {
//                // ✅ Uses username/password if running locally
//                services.AddDbContext<AppDbContext>(options =>
//                    options.UseSqlServer(connectionString));
//            }

//            return services;
//        }
//    }
//}






using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Microsoft.Data.SqlClient;

namespace Backend.Extensions
{
    public static class EFCoreExtensions
    {
        public static IServiceCollection InjectDbContext(
            this IServiceCollection services,
            IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            // ✅ Check if running in Azure (Managed Identity is enabled)
            if (Environment.GetEnvironmentVariable("AZURE_SQL") == "true")
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(new SqlConnection(connectionString)
                    {
                        AccessToken = new DefaultAzureCredential().GetToken(
                            new Azure.Core.TokenRequestContext(new[] { "https://database.windows.net/.default" })
                        ).Token
                    })
                );
            }
            else
            {
                // ✅ Use standard authentication for local development
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }

            return services;
        }
    }
}
