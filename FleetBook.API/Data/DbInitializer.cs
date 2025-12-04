using FleetBook.Models;
using Microsoft.EntityFrameworkCore; 
namespace FleetBook.API.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // 🔹 zamiast Migrate() użyj EnsureCreated()
        context.Database.EnsureCreated();

        // Role
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role { Name = "Admin" },
                new Role { Name = "User" }
            );
            context.SaveChanges();
        }

        // Admin
        if (!context.Users.Any(u => u.Email == "admin@fleetbook.com"))
        {
            var adminRole = context.Roles.First(r => r.Name == "Admin");

            var admin = new User
            {
                Imie = "Admin",
                Nazwisko = "FleetBook",
                Email = "admin@fleetbook.com",
                NumerTelefonu = "123456789",
                Uprawniony = 1,
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("Admin123!", 13),
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(admin);
            context.SaveChanges();

            context.UserRoles.Add(new UserRole
            {
                UserId = admin.Id,
                RoleId = adminRole.Id
            });
            context.SaveChanges();
        }

        // Zwykły user
        if (!context.Users.Any(u => u.Email == "user@fleetbook.com"))
        {
            var userRole = context.Roles.First(r => r.Name == "User");

            var user = new User
            {
                Imie = "Jan",
                Nazwisko = "Kowalski",
                Email = "user@fleetbook.com",
                NumerTelefonu = "987654321",
                Uprawniony = 1,
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword("User123!", 13),
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            context.SaveChanges();

            context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = userRole.Id
            });

            context.SaveChanges();
        }

        // Samochody
        if (!context.Cars.Any())
        {
            var cars = new List<Car>
            {
                new Car
                {
                    Marka = "Toyota",
                    Model = "Corolla",
                    Rejestracja = "WX12345",
                    Rok = 2020,
                    Dostepny = true
                },
                new Car
                {
                    Marka = "Skoda",
                    Model = "Octavia",
                    Rejestracja = "PO98765",
                    Rok = 2019,
                    Dostepny = true
                },
                new Car
                {
                    Marka = "Ford",
                    Model = "Focus",
                    Rejestracja = "KR45678",
                    Rok = 2018,
                    Dostepny = false
                }
            };

            context.Cars.AddRange(cars);
            context.SaveChanges();
        }
    }
}
