using System;
using System.Collections.Generic;

namespace AuthApp.application.DTOs;

public class UpdateServiceRequestDto
{
    public string Customer { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public List<UserAssignmentDto> UserAssignments { get; set; } = new();
}

public class UserAssignmentDto
{
    public Guid UserId { get; set; }
    public bool IsPrimaryAssignee { get; set; }
} 