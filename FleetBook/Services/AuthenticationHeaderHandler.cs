using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace FleetBook.Services;

public class AuthenticationHeaderHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public AuthenticationHeaderHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsStringAsync("authToken");
        
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"🔍 AuthenticationHeaderHandler: Token added to request");
        }
        else
        {
            Console.WriteLine($"🔍 AuthenticationHeaderHandler: No token found");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
