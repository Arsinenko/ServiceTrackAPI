using AuthApp.application.DTOs;

namespace AuthApp.application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto updateUserDto);
    Task<List<UserDto>> UpdateBulkAsync(UpdateBulkUserDto updateBulkUserDto);
    Task<bool> DeleteAsync(Guid id);
} 