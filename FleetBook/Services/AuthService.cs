using FleetBook.Models;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace FleetBook.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    
    // 🔹 Przechowuj token w pamięci, żeby był zawsze dostępny
    private static string? _cachedAccessToken = null;
    private static string? _cachedRefreshToken = null;

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
        Console.WriteLine($"🔍 AuthService.LoginAsync: Sending login request for {email}");
        
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        Console.WriteLine($"🔍 AuthService.LoginAsync: Response status = {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"🔍 AuthService.LoginAsync: Login failed, status = {response.StatusCode}");
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Console.WriteLine($"🔍 AuthService.LoginAsync: Result = {result?.Success}, Token = {(result?.AccessToken?.Length ?? 0)} chars");

        if (result == null || !result.Success)
        {
            Console.WriteLine($"🔍 AuthService.LoginAsync: result is null or Success=false");
            return null;
        }

        if (!string.IsNullOrEmpty(result.AccessToken))
        {
            Console.WriteLine($"🔍 AuthService.LoginAsync: Saving token (length={result.AccessToken.Length})");
            
            await _localStorage.SetItemAsync("accessToken", result.AccessToken);
            await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);
            
            _cachedAccessToken = result.AccessToken;
            _cachedRefreshToken = result.RefreshToken;
            
            Console.WriteLine($"🔍 AuthService.LoginAsync: Token saved to cache, _cachedAccessToken length = {_cachedAccessToken?.Length}");
        }

        return result.User;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"🔍 Login error: {ex.Message}");
        return null;
    }
}


    public async Task<string> GetAccessTokenAsync()
    {
        // 🔹 Najpierw sprawdź cache w pamięci
        if (!string.IsNullOrWhiteSpace(_cachedAccessToken))
        {
            return _cachedAccessToken;
        }

        // Jeśli cache pusty, spróbuj z LocalStorage
        try
        {
            var token = await _localStorage.GetItemAsync<string>("accessToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                _cachedAccessToken = token;
                return token;
            }
        }
        catch
        {
            // LocalStorage może wyrzucić wyjątek, ignoruj
        }

        return string.Empty;
    }

    public async Task LogoutAsync()
    {
        _cachedAccessToken = null;
        _cachedRefreshToken = null;
        
        await _localStorage.RemoveItemAsync("accessToken");
        await _localStorage.RemoveItemAsync("refreshToken");
    }
}
