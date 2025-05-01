using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthApp.application.DTOs;

public class ServiceRequestDto
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public string Customer { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<AssignedUserDto> AssignedUsers { get; set; } = new();

    public static ServiceRequestDto FromServiceRequest(AuthApp.domain.Entities.ServiceRequest request)
    {
        return new ServiceRequestDto
        {
            Id = request.Id,
            ContractId = request.ContractId,
            Customer = request.Customer,
            Description = request.Description,
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt,
            IsCompleted = request.IsCompleted,
            CompletedAt = request.CompletedAt,
            AssignedUsers = request.UserServiceRequests?
                .Select(usr => new AssignedUserDto
                {
                    UserId = usr.UserId,
                    FullName = $"{usr.User.FirstName} {usr.User.LastName}",
                    AssignedAt = usr.AssignedAt,
                    IsPrimaryAssignee = usr.IsPrimaryAssignee
                })
                .ToList() ?? new List<AssignedUserDto>()
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