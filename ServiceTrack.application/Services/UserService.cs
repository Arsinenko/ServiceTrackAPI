using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
namespace AuthApp.application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? UserDto.FromUser(user) : null;
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null ? UserDto.FromUser(user) : null;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(UserDto.FromUser);
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return null;

        var dtoProperties = typeof(UpdateUserDto).GetProperties();
        foreach (var dtoProperty in dtoProperties)
        {
            var value = dtoProperty.GetValue(updateUserDto);
            if (value != null && dtoProperty.Name != nameof(UpdateUserDto.NewPassword))
            {
                var entityProperty = user.GetType().GetProperty(dtoProperty.Name);
                if (entityProperty != null && entityProperty.CanWrite)
                {
                    entityProperty.SetValue(user, value);
                }
            }
        }
        // Смена пароля
        if (!string.IsNullOrEmpty(updateUserDto.NewPassword))
        {
            user.PasswordHash = _passwordHasher.HashPassword(updateUserDto.NewPassword);
        }
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        return UserDto.FromUser(user);
    }

    public Task<List<UserDto>> UpdateBulkAsync(UpdateBulkUserDto updateBulkUserDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _userRepository.SoftDeleteAsync(id);
    }
} 