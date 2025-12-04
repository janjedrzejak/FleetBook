using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FleetBook.Models;

namespace FleetBook.Services;

public class CarApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public CarApiService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<List<CarDto>> GetCarsAsync()
{
    var token = await _authService.GetAccessTokenAsync();
    
    Console.WriteLine($"🔍 CarApiService: token = {(string.IsNullOrWhiteSpace(token) ? "EMPTY" : token.Substring(0, 20) + "...")}");
    Console.WriteLine($"🔍 CarApiService: BaseAddress = {_httpClient.BaseAddress}");

    if (!string.IsNullOrWhiteSpace(token))
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine("🔍 CarApiService: Authorization header set");
    }

    try
    {
        Console.WriteLine("🔍 CarApiService: Calling GET api/cars");
        var response = await _httpClient.GetAsync("api/cars");
        Console.WriteLine($"🔍 CarApiService: Response status = {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"🔍 CarApiService: Error content = {content}");
            return new List<CarDto>();
        }

        var cars = await response.Content.ReadFromJsonAsync<List<CarDto>>();
        Console.WriteLine($"🔍 CarApiService: Got {cars?.Count ?? 0} cars");
        return cars ?? new List<CarDto>();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"🔍 CarApiService: Exception = {ex.Message}");
        return new List<CarDto>();
    }
}

}
