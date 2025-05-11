using System;

namespace AuthApp.application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsAlive { get; set; }
    public RoleDto Role { get; set; }

    public static UserDto FromUser(AuthApp.domain.Entities.User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            CreatedAt = user.CreatedAt.ToLocalTime(),
            LastLoginAt = user.LastLoginAt?.ToLocalTime(),
            UpdatedAt = user.UpdatedAt?.ToLocalTime(),
            IsAlive = user.IsAlive,
            Role = user.Role != null ? RoleDto.FromRole(user.Role) : null
        };
    }
}