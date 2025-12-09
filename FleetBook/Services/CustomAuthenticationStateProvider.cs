using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

namespace FleetBook.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
    private readonly ILocalStorageService _localStorage;

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // 🔹 Zwróć zawsze _currentUser (na starcie będzie niezalogowany)
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public async Task InitializeFromLocalStorageAsync()
    {
        // 🔹 Ta metoda będzie wywołana z OnAfterRenderAsync, gdzie JS interop jest dostępny!
        try
        {
            var token = await _localStorage.GetItemAsStringAsync("authToken");
            
            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"🔓 Token found in localStorage: {token.Substring(0, 20)}...");
                
                var email = await _localStorage.GetItemAsStringAsync("userEmail") ?? "admin@fleetbook.com";
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim("token", token),
                }, "jwt");
                
                _currentUser = new ClaimsPrincipal(identity);
                Console.WriteLine("✅ User restored from token");
                
                // 🔹 Powiadom, że stan się zmienił
                NotifyAuthenticationStateChanged(
                    Task.FromResult(new AuthenticationState(_currentUser)));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Error reading token: {ex.Message}");
        }
    }

    public async Task NotifyUserAuthenticationAsync(string email, string token)
    {
        try
        {
            // Zapisz token
            await _localStorage.SetItemAsStringAsync("authToken", token);
            Console.WriteLine($"💾 Token saved to localStorage");

            // Utwórz claims
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, email),
                new Claim(ClaimTypes.Name, email),
                new Claim("token", token),
            }, "jwt");

            _currentUser = new ClaimsPrincipal(identity);
            Console.WriteLine($"✅ User authenticated: {email}");

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(_currentUser)));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in NotifyUserAuthentication: {ex.Message}");
        }
    }

    public async Task NotifyUserLogoutAsync()
    {
        try
        {
            // Usuń token
            await _localStorage.RemoveItemAsync("authToken");
            Console.WriteLine("🔐 Token removed from localStorage");

            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            Console.WriteLine("✅ User logged out");

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(_currentUser)));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in NotifyUserLogout: {ex.Message}");
        }
    }
}