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

    /// <summary>
    /// Get all roles for a specific user
    /// </summary>
    public async Task<List<Role>> GetUserRolesAsync(int userId)
    {
        return await _db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .ToListAsync();
    }

    /// <summary>
    /// Assign a role to a user
    /// </summary>
    public async Task AssignRoleAsync(int userId, int roleId)
    {
        // Check if user and role exist
        var user = await _db.Users.FindAsync(userId);
        var role = await _db.Roles.FindAsync(roleId);

        if (user == null || role == null)
            throw new InvalidOperationException("User or Role not found");

        // Check if assignment already exists
        var existingAssignment = await _db.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (existingAssignment == null)
        {
            var userRole = new UserRole { UserId = userId, RoleId = roleId };
            _db.UserRoles.Add(userRole);
            await _db.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Remove a role from a user
    /// </summary>
    public async Task RemoveRoleAsync(int userId, int roleId)
    {
        var userRole = await _db.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (userRole != null)
        {
            _db.UserRoles.Remove(userRole);
            await _db.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Update user roles (replaces all existing roles with new ones)
    /// </summary>
    public async Task UpdateUserRolesAsync(int userId, List<int> roleIds)
    {
        // Verify user exists
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        // Remove existing roles
        var existingRoles = _db.UserRoles.Where(ur => ur.UserId == userId);
        _db.UserRoles.RemoveRange(existingRoles);

        // Add new roles
        foreach (var roleId in roleIds)
        {
            var role = await _db.Roles.FindAsync(roleId);
            if (role == null)
                throw new InvalidOperationException($"Role with ID {roleId} not found");

            var userRole = new UserRole { UserId = userId, RoleId = roleId };
            _db.UserRoles.Add(userRole);
        }

        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Get all available roles
    /// </summary>
    public async Task<List<Role>> GetAllRolesAsync()
        => await _db.Roles.ToListAsync();
}