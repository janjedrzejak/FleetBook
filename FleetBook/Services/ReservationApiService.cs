using System.Net.Http.Json;
using FleetBook.Models;

namespace FleetBook.Services;

public interface IReservationApiService
{
    Task<IReadOnlyList<ReservationDto>> GetReservationsAsync(
        DateTime? from = null,
        DateTime? to = null,
        int? carId = null,
        int? userId = null);

    Task<ReservationDto?> GetReservationByIdAsync(int id);

    Task<ReservationDto> CreateReservationAsync(CreateReservationRequest request);

    Task<ReservationDto> ApproveReservationAsync(int id, ApproveReservationRequest request);

    Task<ReservationDto> RejectReservationAsync(int id, RejectReservationRequest request);

    Task CancelReservationAsync(int id);

    Task DeleteReservationAsync(int id);
}

public class ReservationApiService : IReservationApiService
{
    private readonly HttpClient _httpClient;

    public ReservationApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ReservationDto>> GetReservationsAsync(
        DateTime? from = null,
        DateTime? to = null,
        int? carId = null,
        int? userId = null)
    {
        var query = new List<string>();

        if (from.HasValue)
            query.Add($"from={from.Value:O}");
        if (to.HasValue)
            query.Add($"to={to.Value:O}");
        if (carId.HasValue)
            query.Add($"carId={carId.Value}");
        if (userId.HasValue)
            query.Add($"userId={userId.Value}");

        var queryString = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;

        // Dopasuj bazowy URL do backendu (np. /api/reservations)
        var result = await _httpClient.GetFromJsonAsync<List<ReservationDto>>(
            $"api/reservations{queryString}");

        return result ?? new List<ReservationDto>();
    }

    public async Task<ReservationDto?> GetReservationByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<ReservationDto>($"api/reservations/{id}");
    }

    public async Task<ReservationDto> CreateReservationAsync(CreateReservationRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/reservations", request);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<ReservationDto>();
        return created ?? throw new InvalidOperationException("Brak treści odpowiedzi przy tworzeniu rezerwacji");
    }

    public async Task<ReservationDto> ApproveReservationAsync(int id, ApproveReservationRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/reservations/{id}/approve", request);
        response.EnsureSuccessStatusCode();

        var updated = await response.Content.ReadFromJsonAsync<ReservationDto>();
        return updated ?? throw new InvalidOperationException("Brak treści odpowiedzi przy akceptacji rezerwacji");
    }

    public async Task<ReservationDto> RejectReservationAsync(int id, RejectReservationRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/reservations/{id}/reject", request);
        response.EnsureSuccessStatusCode();

        var updated = await response.Content.ReadFromJsonAsync<ReservationDto>();
        return updated ?? throw new InvalidOperationException("Brak treści odpowiedzi przy odrzuceniu rezerwacji");
    }

    public async Task CancelReservationAsync(int id)
    {
        var response = await _httpClient.PostAsync($"api/reservations/{id}/cancel", content: null);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteReservationAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/reservations/{id}");
        response.EnsureSuccessStatusCode();
    }
}