using AuthApp.application.Interfaces;
using AuthApp.application.Services;
using AuthApp.infrastructure.Data;
using AuthApp.infrastructure.Repositories;
using AuthApp.infrastructure.Security;
using AuthApp.infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthApp.infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name)));

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
        services.AddScoped<IJobTypeRepository, JobTypeRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IInspectionMethodRepository, InspectionMethodRepository>();

        // Register services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IServiceRequestService, ServiceRequestService>();
        services.AddScoped<IJobTypeService, JobTypeService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IInspectionMethodService, InspectionMethodService>();
        

        // Register security services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtGenerator, JwtGenerator>();
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        
        return services;
    }
} 