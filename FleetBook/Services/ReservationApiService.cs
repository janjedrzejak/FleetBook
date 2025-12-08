using FleetBook.Models;
using System.Net.Http.Json;

namespace FleetBook.Services;

public class ReservationApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReservationApiService> _logger;

    public ReservationApiService(HttpClient httpClient, ILogger<ReservationApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    // Reservations
    public async Task<List<ReservationDto>> GetReservationsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/reservations");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ReservationDto>>() ?? new();
            }
            _logger.LogError($"Error: {response.StatusCode}");
            return new();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return new();
        }
    }

    public async Task<ReservationDto?> GetReservationByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/reservations/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ReservationDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ReservationDto>> GetUserReservationsAsync(int userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/reservations/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ReservationDto>>() ?? new();
            }
            return new();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return new();
        }
    }

    public async Task<List<ReservationDto>> GetPendingReservationsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/reservations/pending");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ReservationDto>>() ?? new();
            }
            return new();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return new();
        }
    }

    public async Task<bool> CreateReservationAsync(CreateReservationRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/reservations", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ApproveReservationAsync(int id, string? notatki = null)
    {
        try
        {
            var request = new ApproveReservationRequest { Notatki = notatki };
            var response = await _httpClient.PutAsJsonAsync($"api/reservations/{id}/approve", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RejectReservationAsync(int id, string? powod = null)
    {
        try
        {
            var request = new RejectReservationRequest { Powod = powod };
            var response = await _httpClient.PutAsJsonAsync($"api/reservations/{id}/reject", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CancelReservationAsync(int id)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/reservations/{id}/cancel", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteReservationAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/reservations/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return false;
        }
    }

    // Cars
    public async Task<List<CarDto>> GetCarsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/cars");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CarDto>>() ?? new();
            }
            return new();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return new();
        }
    }

    public async Task<List<CarDto>> GetAvailableCarsAsync(DateTime dataOd, DateTime dataDo)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/cars/available?dataOd={dataOd:O}&dataDo={dataDo:O}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CarDto>>() ?? new();
            }
            return new();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return new();
        }
    }

    public async Task<CarDto?> GetCarByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/cars/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CarDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CreateCarAsync(CarDto car)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/cars", car);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateCarAsync(int id, CarDto car)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/cars/{id}", car);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteCarAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/cars/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return false;
        }
    }
}