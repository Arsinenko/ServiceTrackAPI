using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;

namespace AuthApp.application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IRoleRepository _roleRepository;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtGenerator jwtGenerator,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtGenerator = jwtGenerator;
        _roleRepository = roleRepository;
    }

    public async Task<AuthResult> RegisterAsync(RegisterUserDto registerUserDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(registerUserDto.Email);
        if (existingUser != null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Email already exists"
            };
        }

        // Get default role (you might want to make this configurable)
        var defaultRole = await _roleRepository.GetByNameAsync("User");
        if (defaultRole == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Default role not found"
            };
        }

        string passwordHash = _passwordHasher.HashPassword(registerUserDto.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = registerUserDto.Email,
            PasswordHash = passwordHash,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
            CreatedAt = DateTime.UtcNow,
            RoleId = defaultRole.Id
        };
        await _userRepository.CreateAsync(user);
        string token = _jwtGenerator.CreateToken(user);

        return new AuthResult
        {
            Success = true,
            Token = token,
            Email = user.Email,
            Message = "Registration success",
            User = UserDto.FromUser(user)
        };
    }

    public async Task<AuthResult> LoginAsync(LoginUserDto loginDto)
    {
        User? user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Email not exists"
            };
        }

        bool isPasswordValid = _passwordHasher.VerifyHashedPassword(user.PasswordHash, loginDto.Password);

        if (!isPasswordValid)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Invalid Email or password"
            };
        }
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        
        string token = _jwtGenerator.CreateToken(user);

        return new AuthResult
        {
            Success = true,
            Token = token,
            Email = user.Email,
            Message = "Login success",
            User = UserDto.FromUser(user)
        };
    }
}