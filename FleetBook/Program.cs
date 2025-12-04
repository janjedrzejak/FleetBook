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

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5056")
});

// Custom provider + rejestracja pod interfejsem
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthenticationStateProvider>());

// Autoryzacja + stan uwierzytelnienia jako kaskada
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState(); // ← DODAJ TO

builder.Services.AddScoped<AuthService>();

builder.Services.AddSingleton<CustomAuthenticationStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthenticationStateProvider>());

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CarApiService>(); // ← DODAJ TO


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
