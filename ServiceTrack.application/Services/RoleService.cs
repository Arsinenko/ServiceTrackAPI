using AuthApp.application.DTOs;
using AuthApp.application.Exceptions;
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

    private void ValidateRoleNameAndDescription(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new RoleValidationException("Role name cannot be empty");
        
        if (name.Length > 50)
            throw new RoleValidationException("Role name cannot be longer than 50 characters");
        
        if (string.IsNullOrWhiteSpace(description))
            throw new RoleValidationException("Role description cannot be empty");
        
        if (description.Length > 200)
            throw new RoleValidationException("Role description cannot be longer than 200 characters");
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto createRoleDto)
    {
        ValidateRoleNameAndDescription(createRoleDto.Name, createRoleDto.Description);

        // Check if role with this name already exists
        var existingRole = await _roleRepository.GetByNameAsync(createRoleDto.Name);
        if (existingRole != null)
        {
            throw new RoleNameAlreadyExistsException($"Role with name '{createRoleDto.Name}' already exists");
        }

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
        var seenNames = new HashSet<string>();
        var duplicateNames = new HashSet<string>();

        // First pass: Check for duplicates within the batch
        foreach (var dto in createRoleBulkDto.Roles)
        {
            if (!seenNames.Add(dto.Name))
            {
                duplicateNames.Add(dto.Name);
            }
        }
        // Add all duplicates to failedRoles/failureReasons
        foreach (var dto in createRoleBulkDto.Roles)
        {
            if (duplicateNames.Contains(dto.Name))
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
            if (duplicateNames.Contains(dto.Name))
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
        ValidateRoleNameAndDescription(updateRoleDto.Name, updateRoleDto.Description);

        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
            throw new RoleNotFoundException($"Role with id '{id}' not found");

        // Check if we're changing the name and if the new name already exists
        if (role.Name != updateRoleDto.Name)
        {
            var existingRole = await _roleRepository.GetByNameAsync(updateRoleDto.Name);
            if (existingRole != null)
            {
                throw new RoleNameAlreadyExistsException($"Role with name '{updateRoleDto.Name}' already exists");
            }
        }

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
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
            throw new RoleNotFoundException($"Role with id '{id}' not found");

        // Check if role is being used by any users
        if (role.Users != null && role.Users.Any())
        {
            throw new RoleInUseException($"Cannot delete role '{role.Name}' because it is assigned to {role.Users.Count} users");
        }

        await _roleRepository.DeleteAsync(id);
    }

    public async Task<DeleteRoleBulkResultDto> DeleteBulkAsync(IEnumerable<Guid> roleIds)
    {
        var roleIdsList = roleIds.ToList();
        if (!roleIdsList.Any())
        {
            return new DeleteRoleBulkResultDto
            {
                DeletedRoles = new List<RoleDto>(),
                FailedRoleIds = new List<Guid>(),
                FailureReasons = new List<string>()
            };
        }

        var deletedRoles = new List<RoleDto>();
        var failedRoles = new List<Guid>();
        var failureReasons = new List<string>();

        // Get all roles in a single query
        var roles = await _roleRepository.GetByIdsAsync(roleIdsList);
        var rolesDict = roles.ToDictionary(r => r.Id);

        foreach (var roleId in roleIdsList)
        {
            if (!rolesDict.TryGetValue(roleId, out var role))
            {
                failedRoles.Add(roleId);
                failureReasons.Add($"Role with id '{roleId}' not found");
                continue;
            }

            if (role.Users != null && role.Users.Any())
            {
                failedRoles.Add(roleId);
                failureReasons.Add($"Cannot delete role '{role.Name}' because it is assigned to {role.Users.Count} users");
                continue;
            }

            try
            {
                await _roleRepository.DeleteAsync(roleId);
                deletedRoles.Add(RoleDto.FromRole(role));
            }
            catch (Exception ex)
            {
                failedRoles.Add(roleId);
                failureReasons.Add($"Failed to delete role '{role.Name}': {ex.Message}");
            }
        }

        return new DeleteRoleBulkResultDto
        {
            DeletedRoles = deletedRoles,
            FailedRoleIds = failedRoles,
            FailureReasons = failureReasons
        };
    }
}