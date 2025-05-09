using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;

namespace AuthApp.application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        return role != null ? RoleDto.FromRole(role) : null;
    }

    public async Task<RoleDto?> GetByNameAsync(string name)
    {
        var role = await _roleRepository.GetByNameAsync(name);
        return role != null ? RoleDto.FromRole(role) : null;
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return roles.Select(RoleDto.FromRole);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto createRoleDto)
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = createRoleDto.Name,
            Description = createRoleDto.Description
        };

        await _roleRepository.CreateAsync(role);
        return RoleDto.FromRole(role);
    }

    public async Task<IEnumerable<RoleDto>> CreateBulkAsync(CreateRoleBulkDto createRoleBulkDto )
    {
        var roles = createRoleBulkDto.Roles.Select(dto => new Role
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        }).ToList();
        await _roleRepository.CreateBulkAsync(roles);
        return roles.Select(RoleDto.FromRole);
    }

    public async Task<RoleDto?> UpdateAsync(Guid id, UpdateRoleDto updateRoleDto)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
            return null;

        role.Name = updateRoleDto.Name;
        role.Description = updateRoleDto.Description;

        await _roleRepository.UpdateAsync(role);
        return RoleDto.FromRole(role);
    }

    public async Task<List<RoleDto>?> UpdateBulkAsync(UpdateRoleBulkDto updateRoleBulkDto)
    {
        var roles = new List<Role>();
        foreach (var roleDto in updateRoleBulkDto.Roles)
        {
            var role = new Role
            {
                Id = roleDto.Id,
                Name = roleDto.Name,
                Description = roleDto.Description
            };
            roles.Add(role);
        }
        var ids = await _roleRepository.UpdateBulkAsync(roles);
        if  (ids == null)
            return null;
        var updatedRoles = new List<Role>();
        foreach (var id in ids)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return null;
            updatedRoles.Add(role);
        }
        return updatedRoles.Select(RoleDto.FromRole).ToList();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _roleRepository.DeleteAsync(id);
    }
} 