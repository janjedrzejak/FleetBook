using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace FleetBook.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity()); // domyślnie anonimowy

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Zwraca aktualnego użytkownika (zbudowanego w InitializeFromLocalStorageAsync lub NotifyUserAuthenticationAsync)
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public async Task InitializeFromLocalStorageAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsStringAsync("authToken");
            Console.WriteLine($"🔍 InitializeFromLocalStorageAsync: token = {(string.IsNullOrEmpty(token) ? "EMPTY" : token.Substring(0, 20) + "...")}");

            if (!string.IsNullOrEmpty(token))
            {
                var claims = ParseJwtClaims(token);
                var identity = new ClaimsIdentity(claims, "jwt");
                _currentUser = new ClaimsPrincipal(identity);

                Console.WriteLine($"✅ InitializeFromLocalStorageAsync: claims = {_currentUser.Claims.Count()}");

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
            }
            else
            {
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ InitializeFromLocalStorageAsync error: {ex.Message}");
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }

    public async Task NotifyUserAuthenticationAsync(string email, string token)
    {
        // Zapis tokena do localStorage
        await _localStorage.SetItemAsStringAsync("authToken", token);
        await _localStorage.SetItemAsStringAsync("userEmail", email);

        var claims = ParseJwtClaims(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        _currentUser = new ClaimsPrincipal(identity);

        Console.WriteLine($"✅ NotifyUserAuthenticationAsync: IsAuthenticated = {_currentUser.Identity?.IsAuthenticated}, Claims = {_currentUser.Claims.Count()}");

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public async Task NotifyUserLogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("userEmail");

        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        Console.WriteLine("✅ NotifyUserLogoutAsync: user logged out");
    }

    private List<Claim> ParseJwtClaims(string token)
    {
        var claims = new List<Claim>();
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                Console.WriteLine("❌ Invalid JWT format");
                return claims;
            }

            var payload = parts[1];
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

            var bytes = Convert.FromBase64String(payload);
            var json = Encoding.UTF8.GetString(bytes);

            using var doc = JsonDocument.Parse(json);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                var value = prop.Value.ToString();
                claims.Add(new Claim(prop.Name, value));
                Console.WriteLine($"  📌 Claim: {prop.Name} = {value}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ParseJwtClaims error: {ex.Message}");
        }

        return claims;
    }
}
