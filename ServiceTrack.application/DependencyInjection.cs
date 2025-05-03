using AuthApp.application.Interfaces;
using AuthApp.application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuthApp.application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IServiceRequestService, ServiceRequestService>();
        
        return services;
    }
} 