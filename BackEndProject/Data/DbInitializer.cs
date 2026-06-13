using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context, IAuthService authService)
        {
            try
            {
                // Apply any pending migrations
                context.Database.Migrate();

                // Check if users already exist
                if (context.Users.Any())
                {
                    return; // Database already seeded
                }

                // Create default users
                var users = new User[]
                {
                    new User
                    {
                        Username = "manager",
                        Email = "manager@example.com",
                        PasswordHash = authService.HashPassword("Manager@123"),
                        Role = "Manager",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "admin",
                        Email = "admin@example.com",
                        PasswordHash = authService.HashPassword("Admin@123"),
                        Role = "Admin",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "user",
                        Email = "user@example.com",
                        PasswordHash = authService.HashPassword("User@123"),
                        Role = "User",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                foreach (User user in users)
                {
                    context.Users.Add(user);
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log error but don't crash the app
                Console.WriteLine($"Error seeding database: {ex.Message}");
            }
        }
    }
}

