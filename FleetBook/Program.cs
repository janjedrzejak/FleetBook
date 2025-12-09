using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;        
using FleetBook.Services;
using FleetBook.Components;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();

// 👇 DODAJ: DelegatingHandler dla automatycznego wysyłania tokena
builder.Services.AddScoped<AuthenticationHeaderHandler>();

builder.Services.AddScoped(sp => 
{
    var handler = sp.GetRequiredService<AuthenticationHeaderHandler>();
    handler.InnerHandler = new HttpClientHandler();
    
    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:5056")
    };
    return httpClient;
});


// Custom provider + rejestracja pod interfejsem
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthenticationStateProvider>());

// Autoryzacja + stan uwierzytelnienia jako kaskada
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// API Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ICarApiService, CarApiService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<IReservationApiService, ReservationApiService>();




var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
