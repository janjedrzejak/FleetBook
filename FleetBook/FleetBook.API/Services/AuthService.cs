using FleetBook.Models;
using FleetBook.API.Data;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace FleetBook.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(password, user.PasswordHash))
            return null;

        if (user.Uprawniony == 0)
            return null;

        return user;
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
