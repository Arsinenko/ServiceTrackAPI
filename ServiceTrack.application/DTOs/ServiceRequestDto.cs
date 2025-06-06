using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthApp.application.DTOs;

public class ServiceRequestDto
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public required string RequestNumber { get; set; }
    public CustomerDto Customer { get; set; }
    public string Reasons { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime PlannedCompletionDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public JobTypeDto JobType { get; set; }
    public List<AssignedUserDto> AssignedUsers { get; set; } = new();
    public List<AssignedEquipmentDto> AssignedEquipment { get; set; } = new();

    public static ServiceRequestDto FromServiceRequest(AuthApp.domain.Entities.ServiceRequest request)
    {
        return new ServiceRequestDto
        {
            Id = request.Id,
            ContractId = request.ContractId,
            RequestNumber = request.RequestNumber,
            Customer = CustomerDto.FromCustomer(request.Customer),
            Reasons = request.Reasons,
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt,
            PlannedCompletionDate = request.PlannedCompletionDate.ToLocalTime(),
            IsCompleted = request.IsCompleted,
            CompletedAt = request.CompletedAt,
            JobType = request.JobType != null ? JobTypeDto.FromJobType(request.JobType) : null,
            AssignedUsers = request.UserServiceRequests?
                .Select(usr => new AssignedUserDto
                {
                    UserId = usr.UserId,
                    FullName = $"{usr.User.FirstName} {usr.User.LastName}",
                    AssignedAt = usr.AssignedAt,
                    IsPrimaryAssignee = usr.IsPrimaryAssignee
                })
                .ToList() ?? new List<AssignedUserDto>(),
            AssignedEquipment = request.ServiceRequestEquipments?
                .Select(sre => new AssignedEquipmentDto
                {
                    EquipmentId = sre.EquipmentId,
                    Name = sre.Equipment.Name,
                    Model = sre.Equipment.Model,
                    SerialNumber = sre.Equipment.SerialNumber,
                    AddedAt = sre.AddedAt,
                    Notes = sre.Notes,
                    Components = sre.Equipment.Components?
                        .Select(c => new EquipmentDto
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Model = c.Model,
                            SerialNumber = c.SerialNumber,
                            Manufacturer = c.Manufacturer,
                            Quantity = c.Quantity,
                            ParentId = c.ParentId,
                            Description = c.Description,
                            CreatedAt = c.CreatedAt,
                            UpdatedAt = c.UpdatedAt,
                            Components = c.Components?.Select(comp => EquipmentDto.FromEquipment(comp))
                                .ToList(),
                            Attachments = c.Attachments?
                                              .Select(EquipmentAttachmentDto.FromEquipmentAttachment)
                                              .ToList() ??
                                          new List<EquipmentAttachmentDto>(),
                            InspectionMethods = c.EquipmentInspectionMethods
                                .Select(eim => new InspectionMethodDto
                                {
                                    Code = eim.InspectionMethod.Code,
                                    Name = eim.InspectionMethod.Name,
                                    IsAlive = eim.InspectionMethod.IsAlive
                                })
                                .ToList(),
                            SecurityLevel = SecurityLevelDto.FromSecurityLevel(c.SecurityLevel),
                        })
                        .ToList() ?? new List<EquipmentDto>()
                })
                .ToList() ?? new List<AssignedEquipmentDto>()
        };
    }
}

public class AssignedUserDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public DateTime AssignedAt { get; set; }
    public bool IsPrimaryAssignee { get; set; }
}

public class AssignedEquipmentDto
{
    public Guid EquipmentId { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public string SerialNumber { get; set; }
    public List<EquipmentDto> Components { get; set; }
    public DateTime AddedAt { get; set; }
    public string? Notes { get; set; }
}