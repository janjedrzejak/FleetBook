using FleetBook.Models;
using BCrypt.Net;
using FleetBook.API.Data;

namespace FleetBook.API.Data 
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { Name = "Admin" },
                    new Role { Name = "User" }
                );
                context.SaveChanges();
            }

            if (!context.Users.Any(u => u.Email == "admin@fleetbook.com"))
            {
                var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
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

            if (!context.Users.Any(u => u.Email == "user@fleetbook.com"))
            {
                var userRole = context.Roles.FirstOrDefault(r => r.Name == "User");
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
        }
    }
}