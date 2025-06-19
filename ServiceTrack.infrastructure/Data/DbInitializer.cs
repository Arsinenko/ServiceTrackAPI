using AuthApp.domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AuthApp.application.Services;

namespace AuthApp.infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Description = "Administrator with full access",
                    CreatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "User",
                    Description = "Regular user with limited access",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // Create admin user if it doesn't exist
        if (!await context.Users.AnyAsync(u => u.Email == "admin@gmail.com"))
        {
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole != null)
            {
                var passwordHasher = new PasswordHasher();
                var hashedPassword = passwordHasher.HashPassword("P@ssw0rd");

                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Admin",
                    LastName = "Admin",
                    Email = "admin@gmail.com",
                    PasswordHash = hashedPassword,
                    RoleId = adminRole.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsAlive = true
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }

        // Seed security levels
        if (!await context.SecurityLevels.AnyAsync())
        {
            var securityLevels = new List<SecurityLevel>
            {
                new SecurityLevel { Code = "С", Name = "С", Description = "Секретно", IsAlive = true },
                new SecurityLevel { Code = "СС", Name = "СС", Description = "Совершенно секретно", IsAlive = true },
                new SecurityLevel { Code = "ОВ", Name = "ОВ", Description = "Особой важности", IsAlive = true }
            };

            await context.SecurityLevels.AddRangeAsync(securityLevels);
            await context.SaveChangesAsync();
        }

        // Seed inspection methods
        if (!await context.InspectionMethods.AnyAsync())
        {
            var inspectionMethods = new List<InspectionMethod>
            {
                new InspectionMethod { Code = "ДК", Name = "ДК", Description = "ДК", IsAlive = true },
                new InspectionMethod { Code = "СРК", Name = "СРК", Description = "СРК", IsAlive = true },
                new InspectionMethod { Code = "ВОК", Name = "ВОК", Description = "ВОК", IsAlive = true }
            };

            await context.InspectionMethods.AddRangeAsync(inspectionMethods);
            await context.SaveChangesAsync();
        }
    }
} 