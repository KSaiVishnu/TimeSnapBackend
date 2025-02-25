using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Caching.Memory;

namespace Backend.Controllers
{
    public class UserRegistrationModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public required string Role { get; set; }
    }

    public class LoginModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class OtpVerificationDto
    {
        public required string Email { get; set; }
        public required string Otp { get; set; }
    }

    public class OtpRequestModel
    {
        public required string Email { get; set; }
    }

    public class Messager
    {
        public string Message { get; set; }
        public Messager(string message)
        {
            Message = message;            
        }
    }
    
    public static class IdentityUserEndpoints
    {
        public static string? ApiKey { get; private set; }

        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
        {
            var httpContextAccessor = app.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

            app.MapPost("/signup", CreateUser);
            app.MapPost("/signin", SignIn);
            //app.MapPost("/google-signin", GoogleSignIn);
            app.MapPost("/remove-user", RemoveUser);
            app.MapPost("/find-user", FindUser);
            app.MapPost("/send-otp", SendOtp);
            app.MapPost("/verify-otp", VerifyOtp);

            return app;
        }



        [AllowAnonymous]
        public static Messager VerifyOtp([FromBody] OtpVerificationDto model, IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext; // Get HttpContext

            var storedOtp = httpContext.Session.GetString("Otp");
            var storedEmail = httpContext.Session.GetString("Email");

            if (storedOtp != null && storedEmail != null && storedOtp == model.Otp && storedEmail == model.Email)
            {
                //return "User verified successfully.";
                return new Messager("User verified successfully.");
            }

            //return "Invalid OTP or email.";
            return new Messager("User verified successfully.");
        }



        //    [AllowAnonymous]
        //    private static async Task SendOtpEmail(string email, string otp)
        //    {
        //        var config = new ConfigurationBuilder()
        //.SetBasePath(AppContext.BaseDirectory) // Set base path
        //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Load config
        //.AddEnvironmentVariables() // Add environment variables
        //.Build();

        //        ApiKey = Environment.GetEnvironmentVariable("MySendGridAPIKey")
        //                 ?? config["SendGrid:ApiKey"];

        //        var client = new SendGridClient(ApiKey);

        //        var from = new EmailAddress("saivishnukamisetty@gmail.com", "Time Snap");
        //        var subject = "Your OTP Code";
        //        var to = new EmailAddress(email);
        //        var plainTextContent = $"Your OTP code is: {otp}. Please enter this code to complete your registration.";
        //        var htmlContent = $"<strong>Your OTP code is: {otp}</strong>. Please enter this code to complete your registration.";
        //        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);


        //        // Send the email
        //        var response = await client.SendEmailAsync(msg);
        //        Console.WriteLine($"SendGrid Response: {response.StatusCode}");
        //        var responseBody = await response.Body.ReadAsStringAsync();
        //        Console.WriteLine($"SendGrid Response Body: {responseBody}");

        //        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        //        {
        //            // Handle error (you can log the error here)
        //            throw new Exception("Failed to send OTP email. SendGrid Response: " + response.StatusCode);
        //        }
        //    }




        [AllowAnonymous]
        private static async Task SendOtpEmail(string email, string otp)
        {
            // Retrieve SendGrid API Key from environment variables
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("SendGrid API Key is missing! Set 'SENDGRID_API_KEY' in environment variables.");
            }

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("saivishnukamisetty@gmail.com", "Time Snap");
            var subject = "Your OTP Code";
            var to = new EmailAddress(email);
            var plainTextContent = $"Your OTP code is: {otp}. Please enter this code to complete your registration.";
            var htmlContent = $"<strong>Your OTP code is: {otp}</strong>. Please enter this code to complete your registration.";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            try
            {
                // Send the email
                var response = await client.SendEmailAsync(msg);
                Console.WriteLine($"SendGrid Response: {response.StatusCode}");

                var responseBody = await response.Body.ReadAsStringAsync();
                Console.WriteLine($"SendGrid Response Body: {responseBody}");

                // Check if email was successfully sent
                if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    throw new Exception($"Failed to send OTP email. SendGrid Response: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending OTP email: {ex.Message}");
                throw; // Rethrow for higher-level handling
            }
        }





        [AllowAnonymous]
        public static async Task<IResult> SendOtp(UserManager<AppUser> userManager, [FromBody] OtpRequestModel body, IHttpContextAccessor httpContextAccessor)
        {
            //var user = await userManager.FindByEmailAsync(email);
            var httpContext = httpContextAccessor.HttpContext; // Get HttpContext
            if (httpContext == null)
            {
                return Results.BadRequest("HttpContext is null.");
            }

            var otp = new Random().Next(100000, 999999).ToString();

            httpContext.Session.SetString("Email", body.Email);
            httpContext.Session.SetString("Otp", otp);

            await SendOtpEmail(body.Email, otp);


            return Results.Ok(body.Email);
        }

        [AllowAnonymous]
        private static async Task<IResult> CreateUser(
            HttpContext httpContext,
            UserManager<AppUser> userManager,
            [FromBody] UserRegistrationModel userRegistrationModel)
        {
            var context = httpContext.RequestServices.GetRequiredService<AppDbContext>();
            var employee = context.UserEmployees
                                    .FirstOrDefault(e => e.UserName == userRegistrationModel.FullName);
            if (employee == null)
            {
                return Results.BadRequest("Employee not found in UserEmployee table.");
            }

            AppUser user = new AppUser()
            {
                UserName = userRegistrationModel.Email,
                Email = userRegistrationModel.Email,
                FullName = userRegistrationModel.FullName,
                EmpId = employee.EmployeeId
            };
            var result = await userManager.CreateAsync(
                user,
                userRegistrationModel.Password);
            await userManager.AddToRoleAsync(user, userRegistrationModel.Role);

            return result.Succeeded ? Results.Ok(result) : Results.BadRequest(result);

        }

        //[AllowAnonymous]
        //private static async Task<IResult> GoogleSignIn(UserManager<AppUser> userManager,
        //    [FromBody] UserRegistrationModel userRegistrationModel)
        //{
        //    AppUser user = new AppUser()
        //    {
        //        UserName = userRegistrationModel.Email,
        //        Email = userRegistrationModel.Email,
        //        FullName = userRegistrationModel.FullName,
        //    };
        //    var result = await userManager.CreateAsync(
        //        user,
        //        userRegistrationModel.Password);
        //    await userManager.AddToRoleAsync(user, userRegistrationModel.Role);

        //    if (result.Succeeded)
        //        return Results.Ok(result);
        //    else
        //        return Results.BadRequest(result);
        //}

        [AllowAnonymous]
        private static async Task<IResult> RemoveUser(UserManager<AppUser> userManager,
                ClaimsPrincipal user,
                [FromBody] dynamic body)
        {
            string userID = user.Claims.First(x => x.Type == "UserID").Value;
            //Console.WriteLine(userID);
            var userDetails = await userManager.FindByIdAsync(userID);
            if (userDetails == null)
                return Results.NotFound("User not found");
            await userManager.DeleteAsync(userDetails);
            return Results.Ok($"Deleted");
        }

        [AllowAnonymous]
        private static async Task<IResult> FindUser(UserManager<AppUser> userManager,
        [FromBody] OtpRequestModel body)
        {
            var user = await userManager.FindByEmailAsync(body.Email);
            if (user != null)
            {
                return Results.Ok($"User Found");
            }
            return Results.NotFound("User not found");
        }


        [AllowAnonymous]
        private static async Task<IResult> SignIn(
            UserManager<AppUser> userManager,
                [FromBody] LoginModel loginModel,
                IOptions<AppSettings> appSettings)
        {
            var user = await userManager.FindByEmailAsync(loginModel.Email);
            //userManager.DeleteAsync(user);
            if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var roles = await userManager.GetRolesAsync(user);
                var signInKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret)
                                );
                ClaimsIdentity claims = new(
                    [
                        new("UserID",user.Id.ToString()),
                        new(ClaimTypes.Role,roles.First()),
                    ]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddDays(10),
                    SigningCredentials = new SigningCredentials(
                        signInKey,
                        SecurityAlgorithms.HmacSha256Signature
                        )
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Results.Ok(new { token });
            }
            else
                return Results.BadRequest(new { message = "Username or password is incorrect." });
        }
    }
}

