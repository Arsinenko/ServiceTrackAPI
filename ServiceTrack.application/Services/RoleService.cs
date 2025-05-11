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

    public async Task<CreateRoleBulkResultDto> CreateBulkAsync(CreateRoleBulkDto createRoleBulkDto)
    {
        var createdRoles = new List<Role>();
        var failedRoles = new List<CreateRoleDto>();
        var failureReasons = new List<string>();
        var duplicateNamesInBatch = new HashSet<string>();

        // First pass: Check for duplicates within the batch
        foreach (var dto in createRoleBulkDto.Roles)
        {
            if (!duplicateNamesInBatch.Add(dto.Name))
            {
                failedRoles.Add(dto);
                failureReasons.Add($"Role name '{dto.Name}' is duplicated in the batch");
            }
        }

        // Get all existing role names in a single query
        var existingRoles = await _roleRepository.GetAllAsync();
        var existingRoleNames = existingRoles.Select(r => r.Name).ToHashSet();

        // Second pass: Check against existing roles and create valid ones
        foreach (var dto in createRoleBulkDto.Roles)
        {
            // Skip roles that failed in the first pass
            if (duplicateNamesInBatch.Contains(dto.Name))
                continue;

            // Check if role with this name already exists
            if (existingRoleNames.Contains(dto.Name))
            {
                failedRoles.Add(dto);
                failureReasons.Add($"Role with name '{dto.Name}' already exists");
                continue;
            }

            // Create new role
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description
            };
            createdRoles.Add(role);
        }

        // Only attempt to create roles if there are any valid ones
        if (createdRoles.Any())
        {
            await _roleRepository.CreateBulkAsync(createdRoles);
        }

        return new CreateRoleBulkResultDto
        {
            CreatedRoles = createdRoles.Select(RoleDto.FromRole).ToList(),
            FailedRoles = failedRoles,
            FailureReasons = failureReasons
        };
    }

    public async Task<RoleDto?> UpdateAsync(Guid id, UpdateRoleDto updateRoleDto)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
            return null;

        role.Name = updateRoleDto.Name;
        role.Description = updateRoleDto.Description;

        var result = await _roleRepository.UpdateAsync(role);
        return RoleDto.FromRole(result);
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
        var result = await _roleRepository.UpdateBulkAsync(roles);
        
        return result.Select(RoleDto.FromRole).ToList();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _roleRepository.DeleteAsync(id);
    }
} 