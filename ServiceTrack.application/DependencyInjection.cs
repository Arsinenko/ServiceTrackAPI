using AuthApp.application.Interfaces;
using AuthApp.application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuthApp.application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEquipmentComponentService, EquipmentComponentService>();
        
        return services;
    }
} 