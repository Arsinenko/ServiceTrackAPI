using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthApp.application.DTOs;

public class CreateServiceRequestDto
{
    [Required(ErrorMessage = "ContractId is required!")]
    public required int ContractId { get; set; }
    [Required(ErrorMessage = "Customer field is required!")]
    public required string Customer { get; set; }
    [Required(ErrorMessage = "Description is required")]
    public required string Description { get; set; }
    [Required(ErrorMessage = "JobType is requred!")]
    public Guid JobTypeId { get; set; }
    [Required]
    public List<InitialUserAssignmentDto> InitialAssignments { get; set; } = new();
    [Required]
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