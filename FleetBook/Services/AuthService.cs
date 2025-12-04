using FleetBook.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace FleetBook.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<UserDto?> LoginAsync(string email, string password)
    {
        try
        {
            var request = new LoginRequest { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (result == null || !result.Success)
                return null;

            if (!string.IsNullOrEmpty(result.AccessToken))
            {
                await _localStorage.SetItemAsync("accessToken", result.AccessToken);
                await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);
            }

            return result.User;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return null;
        }
    }

    public async Task<string> GetAccessTokenAsync()
    {
        try
        {
            return await _localStorage.GetItemAsync<string>("accessToken") ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("accessToken");
        await _localStorage.RemoveItemAsync("refreshToken");
    }
}
