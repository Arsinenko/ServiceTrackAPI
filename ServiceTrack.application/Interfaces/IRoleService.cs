using AuthApp.application.DTOs;

namespace AuthApp.application.Interfaces;

public interface IRoleService
{
    Task<RoleDto?> GetByIdAsync(Guid id);
    Task<RoleDto?> GetByNameAsync(string name);
    Task<IEnumerable<RoleDto>> GetAllAsync();
    Task<RoleDto> CreateAsync(CreateRoleDto createRoleDto);
    Task<CreateRoleBulkResultDto> CreateBulkAsync(CreateRoleBulkDto createRoleBulkDto);
    Task<RoleDto?> UpdateAsync(Guid id, UpdateRoleDto updateRoleDto);
    Task<List<RoleDto>?> UpdateBulkAsync(UpdateRoleBulkDto updateRoleBulkDto);
    Task DeleteAsync(Guid id);
    Task<DeleteRoleBulkResultDto> DeleteBulkAsync(IEnumerable<Guid> roleIds);
} 