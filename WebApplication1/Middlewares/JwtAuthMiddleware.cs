using WebApplication1.Services;
using System.Security.Claims;

namespace Middlewares;

public class JwtAuthMiddleware
{
    private readonly RequestDelegate _next;

    public JwtAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AuthService authService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var userProfile = await authService.GetUserProfileAsync(token);
                if (userProfile != null)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userProfile.Id.ToString()),
                        new Claim(ClaimTypes.Email, userProfile.Email),
                        new Claim(ClaimTypes.Name, userProfile.Email),
                        new Claim("UserId", userProfile.Id.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, "CustomJwt");
                    context.User = new ClaimsPrincipal(identity);
                    context.Items["UserId"] = userProfile.Id;
                    context.Items["UserEmail"] = userProfile.Email;
                    context.Items["IsAuthenticated"] = true;

                    Console.WriteLine($"User authenticated: {userProfile.Email}, ID: {userProfile.Id}");
                }
                else
                {
                    Console.WriteLine("User profile not found for token");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in JWT auth middleware: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("No token provided");
        }

        await _next(context);
    }
}

public static class JwtAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtAuth(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtAuthMiddleware>();
    }
}