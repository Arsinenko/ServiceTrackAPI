using System;
using System.Collections.Generic;

namespace AuthApp.application.DTOs;

public class CreateServiceRequestDto
{
    public required int ContractId { get; set; }
    public required string Customer { get; set; }
    public string Description { get; set; }
    public Guid JobTypeId { get; set; }
    public List<InitialUserAssignmentDto> InitialAssignments { get; set; } = new();
    public List<InitialEquipmentAssignmentDto> InitialEquipment { get; set; } = new();
}

public class InitialUserAssignmentDto
{
    public Guid UserId { get; set; }
    public bool IsPrimaryAssignee { get; set; }
}

public class InitialEquipmentAssignmentDto
{
    public Guid EquipmentId { get; set; }
    public string? Notes { get; set; }
}

public class CreateServiceRequestBulkDto
{
    public required ICollection<CreateServiceRequestDto> ServiceRequests { get; set; }
}

public class DeleteServiceRequestBulkResult
{
    public required ICollection<ServiceRequestDto> DeletedServiceRequests { get; set; }
    public required ICollection<int> FailedServiceRequestIds { get; set; }
    public required ICollection<string> FailureReasons { get; set; }
}