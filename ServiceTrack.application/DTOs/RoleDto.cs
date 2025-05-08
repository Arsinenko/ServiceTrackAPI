namespace AuthApp.application.DTOs;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static RoleDto FromRole(domain.Entities.Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt.ToLocalTime(),
            UpdatedAt = role.UpdatedAt?.ToLocalTime()
        };
    }
}

public class CreateRoleDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}

public class UpdateRoleDto : CreateRoleDto;

public class CreateRoleBulkDto
{
    public required ICollection<RoleDto> Roles;
}