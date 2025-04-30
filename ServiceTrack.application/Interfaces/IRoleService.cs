using AuthApp.application.DTOs;

namespace AuthApp.application.Interfaces;

public interface IRoleService
{
    Task<RoleDto?> GetByIdAsync(Guid id);
    Task<RoleDto?> GetByNameAsync(string name);
    Task<IEnumerable<RoleDto>> GetAllAsync();
    Task<RoleDto> CreateAsync(CreateRoleDto createRoleDto);
    Task<RoleDto?> UpdateAsync(Guid id, UpdateRoleDto updateRoleDto);
    Task DeleteAsync(Guid id);
} 