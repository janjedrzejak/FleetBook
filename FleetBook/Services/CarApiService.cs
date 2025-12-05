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

    private async Task<string?> GetAuthTokenAsync()
    {
        var token = await _authService.GetAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
        return token;
    }

    public async Task<List<CarDto>> GetCarsAsync()
    {
        await GetAuthTokenAsync();

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

    public async Task<CarDto?> GetCarByIdAsync(int id)
    {
        await GetAuthTokenAsync();

        try
        {
            Console.WriteLine($"🔍 CarApiService: Calling GET api/cars/{id}");
            var response = await _httpClient.GetAsync($"api/cars/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"🔍 CarApiService: Error status = {response.StatusCode}");
                return null;
            }

            var car = await response.Content.ReadFromJsonAsync<CarDto>();
            return car;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 CarApiService: Exception = {ex.Message}");
            return null;
        }
    }

    public async Task<bool> AddCarAsync(CarDto car)
    {
        await GetAuthTokenAsync();

        try
        {
            Console.WriteLine($"🔍 CarApiService: Calling POST api/cars");
            var response = await _httpClient.PostAsJsonAsync("api/cars", car);
            Console.WriteLine($"🔍 CarApiService: Response status = {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"🔍 CarApiService: Error content = {content}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 CarApiService: Exception = {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateCarAsync(int id, CarDto car)
    {
        await GetAuthTokenAsync();

        try
        {
            Console.WriteLine($"🔍 CarApiService: Calling PUT api/cars/{id}");
            var response = await _httpClient.PutAsJsonAsync($"api/cars/{id}", car);
            Console.WriteLine($"🔍 CarApiService: Response status = {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"🔍 CarApiService: Error content = {content}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 CarApiService: Exception = {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteCarAsync(int id)
    {
        await GetAuthTokenAsync();

        try
        {
            Console.WriteLine($"🔍 CarApiService: Calling DELETE api/cars/{id}");
            var response = await _httpClient.DeleteAsync($"api/cars/{id}");
            Console.WriteLine($"🔍 CarApiService: Response status = {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"🔍 CarApiService: Error content = {content}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 CarApiService: Exception = {ex.Message}");
            return false;
        }
    }
}