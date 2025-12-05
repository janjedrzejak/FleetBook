using Microsoft.EntityFrameworkCore;
using FleetBook.API.Data;
using FleetBook.Models;

namespace FleetBook.Services;

public class UserService
{
    private readonly ApplicationDbContext _db;

    public UserService(ApplicationDbContext db) => _db = db;

    public async Task<List<User>> GetUsersAsync()
        => await _db.Users.ToListAsync();

    public async Task<User?> GetUserByIdAsync(int id)
        => await _db.Users.FindAsync(id);

    public async Task AddUserAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        var existingUser = await _db.Users.FindAsync(user.Id);
        if (existingUser != null)
        {
            existingUser.Imie = user.Imie;
            existingUser.Nazwisko = user.Nazwisko;
            existingUser.Email = user.Email;
            existingUser.NumerTelefonu = user.NumerTelefonu;
            existingUser.Uprawniony = user.Uprawniony;

            await _db.SaveChangesAsync();
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<List<User>> GetUsersSortedAsync(string sortBy = "id", bool ascending = true)
    {
        var query = _db.Users.AsQueryable();

        query = sortBy.ToLower() switch
        {
            "imie" => ascending ? query.OrderBy(u => u.Imie) : query.OrderByDescending(u => u.Imie),
            "nazwisko" => ascending ? query.OrderBy(u => u.Nazwisko) : query.OrderByDescending(u => u.Nazwisko),
            "email" => ascending ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
            "uprawniony" => ascending ? query.OrderBy(u => u.Uprawniony) : query.OrderByDescending(u => u.Uprawniony),
            _ => ascending ? query.OrderBy(u => u.Id) : query.OrderByDescending(u => u.Id),
        };

        return await query.ToListAsync();
    }
}