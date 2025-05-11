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

        // Get role by ID from the request
        var role = await _roleRepository.GetByIdAsync(registerUserDto.RoleId);
        if (role == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Specified role not found"
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
            RoleId = role.Id,
            Role = role
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

    public async Task<List<AuthResult>> RegisterBulkAsync(List<RegisterUserDto> registerDtos)
    {
        var results = new List<AuthResult>();
        var usersToCreate = new List<User>();
        var existingEmails = new HashSet<string>();

        // First, check all emails for duplicates
        foreach (var dto in registerDtos)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                results.Add(new AuthResult
                {
                    Success = false,
                    Message = $"Email {dto.Email} already exists",
                    Email = dto.Email
                });
                existingEmails.Add(dto.Email);
            }
        }

        // Filter out users with existing emails
        var validDtos = registerDtos.Where(dto => !existingEmails.Contains(dto.Email)).ToList();

        // Get all unique role IDs
        var roleIds = validDtos.Select(dto => dto.RoleId).Distinct().ToList();
        var roles = new Dictionary<Guid, Role>();

        // Fetch all required roles
        foreach (var roleId in roleIds)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                // Add failure result for all users with this role
                foreach (var dto in validDtos.Where(d => d.RoleId == roleId))
                {
                    results.Add(new AuthResult
                    {
                        Success = false,
                        Message = $"Specified role not found for user {dto.Email}",
                        Email = dto.Email
                    });
                }
                continue;
            }
            roles[roleId] = role;
        }

        // Create users for valid DTOs with valid roles
        foreach (var dto in validDtos)
        {
            if (!roles.ContainsKey(dto.RoleId))
                continue; // Skip if role was invalid

            var role = roles[dto.RoleId];
            string passwordHash = _passwordHasher.HashPassword(dto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = passwordHash,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CreatedAt = DateTime.UtcNow,
                RoleId = role.Id,
                Role = role
            };

            usersToCreate.Add(user);
        }

        // Bulk create users if there are any valid ones
        if (usersToCreate.Any())
        {
            var createdUsers = await _userRepository.CreateBulkAsync(usersToCreate);

            // Generate tokens and create success results for created users
            foreach (var user in createdUsers)
            {
                string token = _jwtGenerator.CreateToken(user);
                results.Add(new AuthResult
                {
                    Success = true,
                    Token = token,
                    Email = user.Email,
                    Message = "Registration success",
                    User = UserDto.FromUser(user)
                });
            }
        }

        return results;
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