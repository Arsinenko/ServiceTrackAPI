using System;

namespace AuthApp.application.DTOs;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static RoleDto FromRole(AuthApp.domain.Entities.Role role)
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
    public string Name { get; set; }
    public string Description { get; set; }
}

public class UpdateRoleDto
{
    public string Name { get; set; }
    public string Description { get; set; }
} 