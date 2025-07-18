using System;
using System.Collections.Generic;

namespace AuthApp.application.DTOs;

public class UpdateServiceRequestDto
{
    public int CustomerId { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public Guid JobTypeId { get; set; }
    public List<UserAssignmentDto> UserAssignments { get; set; } = new();
    public List<EquipmentAssignmentDto> EquipmentAssignments { get; set; } = new();
}

public class UpdateServiceRequestBulkItemDto : UpdateServiceRequestDto
{
    public required int Id { get; set; }
}

public class UpdateServiceRequestBulkDto
{
    public required List<UpdateServiceRequestBulkItemDto> Items { get; set; }
} 

public class UserAssignmentDto
{
    public Guid UserId { get; set; }
    public bool IsPrimaryAssignee { get; set; }
}

public class EquipmentAssignmentDto
{
    public Guid EquipmentId { get; set; }
    public string? Notes { get; set; }
} 