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
    public List<EquipmentDto> AssignedEquipment { get; set; } = new();

    public static ServiceRequestDto FromServiceRequest(AuthApp.domain.Entities.ServiceRequest request)
    {
        return new ServiceRequestDto
        {
            Id = request.Id,
            ContractId = request.ContractId,
            RequestNumber = request.RequestNumber,
            Customer = request.Customer != null ? CustomerDto.FromCustomer(request.Customer) : null,
            Reasons = request.Reasons,
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt,
            PlannedCompletionDate = request.PlannedCompletionDate.ToLocalTime(),
            IsCompleted = request.IsCompleted,
            CompletedAt = request.CompletedAt,
            JobType = request.JobType != null ? JobTypeDto.FromJobType(request.JobType) : null,
            AssignedUsers = request.UserServiceRequests?
                .Where(usr => usr != null && usr.User != null)
                .Select(usr => new AssignedUserDto
                {
                    UserId = usr.UserId,
                    FullName = $"{usr.User.FirstName} {usr.User.LastName}",
                    AssignedAt = usr.AssignedAt,
                    IsPrimaryAssignee = usr.IsPrimaryAssignee
                })
                .ToList() ?? new List<AssignedUserDto>(),
            AssignedEquipment = request.ServiceRequestEquipments?
                .Where(sre => sre != null && sre.Equipment != null)
                .Select(sre => EquipmentDto.FromEquipment(sre.Equipment))
                .ToList() ?? new List<EquipmentDto>()
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