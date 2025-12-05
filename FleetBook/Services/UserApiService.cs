using System.Net.Http.Headers;
using System.Net.Http.Json;
using FleetBook.Models;

namespace FleetBook.Services;

public class UserApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public UserApiService(HttpClient httpClient, AuthService authService)
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

    public async Task<List<UserDto>> GetUsersAsync()
    {
        await GetAuthTokenAsync();

        try
        {
            Console.WriteLine("🔍 UserApiService: Calling GET api/users");
            var response = await _httpClient.GetAsync("api/users");
            Console.WriteLine($"🔍 UserApiService: Response status = {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"🔍 UserApiService: Error content = {content}");
                return new List<UserDto>();
            }

            var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
            Console.WriteLine($"🔍 UserApiService: Got {users?.Count ?? 0} users");
            return users ?? new List<UserDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 UserApiService: Exception = {ex.Message}");
            return new List<UserDto>();
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        await GetAuthTokenAsync();

        try
        {
            Console.WriteLine($"🔍 UserApiService: Calling GET api/users/{id}");
            var response = await _httpClient.GetAsync($"api/users/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"🔍 UserApiService: Error status = {response.StatusCode}");
                return null;
            }

            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 UserApiService: Exception = {ex.Message}");
            return null;
        }
    }

    public async Task<bool> AddUserAsync(UserDto user)
    {
        await GetAuthTokenAsync();

        try
        {
            Console.WriteLine($"🔍 UserApiService: Calling POST api/users");
            var response = await _httpClient.PostAsJsonAsync("api/users", user);
            Console.WriteLine($"🔍 UserApiService: Response status = {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"🔍 UserApiService: Error content = {content}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 UserApiService: Exception = {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateUserAsync(int id, UserDto user)
    {
        await GetAuthTokenAsync();

        try
        {
            Console.WriteLine($"🔍 UserApiService: Calling PUT api/users/{id}");
            
            var userToUpdate = new
            {
                user.Id,
                user.Imie,
                user.Nazwisko,
                user.Email,
                user.NumerTelefonu,
                user.Uprawniony,
                PasswordHash = "placeholder_password"
            };
            
            var response = await _httpClient.PutAsJsonAsync($"api/users/{id}", userToUpdate);
            Console.WriteLine($"🔍 UserApiService: Response status = {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"🔍 UserApiService: Error content = {content}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 UserApiService: Exception = {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        await GetAuthTokenAsync();

        try
        {
            Console.WriteLine($"🔍 UserApiService: Calling DELETE api/users/{id}");
            var response = await _httpClient.DeleteAsync($"api/users/{id}");
            Console.WriteLine($"🔍 UserApiService: Response status = {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"🔍 UserApiService: Error content = {content}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 UserApiService: Exception = {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Get all roles for a user
    /// </summary>
    public async Task<List<RoleDto>> GetUserRolesAsync(int userId)
    {
        await GetAuthTokenAsync();

        try
        {
            Console.WriteLine($"🔍 UserApiService: Calling GET api/users/{userId}/roles");
            var response = await _httpClient.GetAsync($"api/users/{userId}/roles");
            Console.WriteLine($"🔍 UserApiService: Response status = {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"🔍 UserApiService: Error status = {response.StatusCode}");
                return new List<RoleDto>();
            }

            var roles = await response.Content.ReadFromJsonAsync<List<RoleDto>>();
            return roles ?? new List<RoleDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 UserApiService: Exception = {ex.Message}");
            return new List<RoleDto>();
        }
    }

    /// <summary>
    /// Update user roles (replaces all roles)
    /// </summary>
    public async Task<bool> UpdateUserRolesAsync(int userId, List<int> roleIds)
    {
        await GetAuthTokenAsync();

        try
        {
            Console.WriteLine($"🔍 UserApiService: Calling PUT api/users/{userId}/roles with {roleIds.Count} roles");
            var response = await _httpClient.PutAsJsonAsync($"api/users/{userId}/roles", roleIds);
            Console.WriteLine($"🔍 UserApiService: Response status = {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"🔍 UserApiService: Error content = {content}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 UserApiService: Exception = {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Get all available roles
    /// </summary>
    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        await GetAuthTokenAsync();

        try
        {
            Console.WriteLine("🔍 UserApiService: Calling GET api/users/_/roles");
            var response = await _httpClient.GetAsync("api/users/_/roles");
            Console.WriteLine($"🔍 UserApiService: Response status = {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"🔍 UserApiService: Error status = {response.StatusCode}");
                return new List<RoleDto>();
            }

            var roles = await response.Content.ReadFromJsonAsync<List<RoleDto>>();
            return roles ?? new List<RoleDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔍 UserApiService: Exception = {ex.Message}");
            return new List<RoleDto>();
        }
    }
}