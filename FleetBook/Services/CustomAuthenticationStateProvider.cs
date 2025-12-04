using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace FleetBook.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public void NotifyUserAuthentication(string email)
    {
        // 🔹 WAŻNE: AuthenticationType MUSI być ustawiony (nie null), żeby IsAuthenticated był true
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, email),
            new Claim(ClaimTypes.Name, email),
        }, "jwt");  // ← "jwt" zamiast "custom", ale każda string działa jeśli nie jest null

        _currentUser = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public void NotifyUserLogout()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser)));
    }
}
