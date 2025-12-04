using FleetBook.Models;
using FleetBook.API.Data;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace FleetBook.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtTokenService _jwtTokenService;

    public AuthService(ApplicationDbContext context, JwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(password, user.PasswordHash))
            return null;

        if (user.Uprawniony == 0)
            return null;

        // 🔹 Wygeneruj tokeny
        var roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>();
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        // 🔹 Zwróć AuthResponse z tokenami
        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            User = new UserDto
            {
                Id = user.Id,
                Imie = user.Imie,
                Nazwisko = user.Nazwisko,
                Email = user.Email
            }
        };
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
    }
}
