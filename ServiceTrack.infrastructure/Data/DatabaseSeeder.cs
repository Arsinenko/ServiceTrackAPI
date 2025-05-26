using AuthApp.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedDataAsync(ApplicationDbContext context)
    {
        if (!await context.SecurityLevels.AnyAsync())
        {
            var securityLevels = new List<SecurityLevel>
            {
                new SecurityLevel { Code = "С", Name = "С", Description = "Секретно", IsAlive = true },
                new SecurityLevel { Code = "СС", Name = "СС", Description = "Совершенно секретно", IsAlive = true },
                new SecurityLevel { Code = "ОВ", Name = "ОВ", Description = "Особой важности", IsAlive = true }
            };

            await context.SecurityLevels.AddRangeAsync(securityLevels);
        }

        if (!await context.InspectionMethods.AnyAsync())
        {
            var inspectionMethods = new List<InspectionMethod>
            {
                new InspectionMethod { Code = "ДК", Name = "ДК", Description = "ДК", IsAlive = true },
                new InspectionMethod { Code = "СРК", Name = "СРК", Description = "СРК", IsAlive = true },
                new InspectionMethod { Code = "ВОК", Name = "ВОК", Description = "ВОК", IsAlive = true }
            };

            await context.InspectionMethods.AddRangeAsync(inspectionMethods);
        }

        await context.SaveChangesAsync();
    }
} 