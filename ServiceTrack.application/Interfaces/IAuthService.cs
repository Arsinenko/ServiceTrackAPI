using AuthApp.application.DTOs;
using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterUserDto registerDto);
    Task<AuthResult> LoginAsync(LoginUserDto loginDto);
}