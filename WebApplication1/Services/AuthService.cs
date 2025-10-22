using System.Net.Http.Json;
using System.Text.Json;
using Models;
namespace WebApplication1.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<LoginResult?> LoginAsync(string email, string password)
    {
        try
        {
            var url = $"{GetAuthBaseUrl()}/api/auth/login";
            Console.WriteLine($"🔍 Calling auth service: {url}");

            var response = await _httpClient.PostAsJsonAsync(url, new { email, password });
            Console.WriteLine($"🔍 Response status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"🔍 Raw response: {content}");

                // Десериализация с правильными настройками
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true  // важно!
                };

                var result = await response.Content.ReadFromJsonAsync<LoginResult>(options);
                Console.WriteLine($"🔍 Parsed result - Token: {result?.access_token != null}, UserId: {result?.user_id}");

                return result;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"🔍 Error: {errorContent}");
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 Exception: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{GetAuthBaseUrl()}/api/auth/register",
                new { email, password });

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration error: {ex.Message}");
            return false;
        }
    }

    public async Task<UserProfile?> GetUserProfileAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{GetAuthBaseUrl()}/api/auth/profile");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserProfile>();
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get profile error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var userProfile = await GetUserProfileAsync(token);
            return userProfile != null;
        }
        catch
        {
            return false;
        }
    }

    private string GetAuthBaseUrl() =>
        _configuration["AuthService:BaseUrl"] ?? "https://localhost:7000";
}