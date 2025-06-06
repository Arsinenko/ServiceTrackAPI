using System.ComponentModel.DataAnnotations;
using AuthApp.domain.Entities;

namespace AuthApp.application.DTOs;

public class EquipmentDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Model { get; set; }
    public required string SerialNumber { get; set; }
    public required string Manufacturer { get; set; }
    public int Category { get; set; }
    public int Quantity { get; set; }
    public Guid? ExecutorId { get; set; }
    public UserDto? Executor { get; set; }
    public string? SZZ { get; set; }
    public Guid? ParentId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<EquipmentDto>? Components { get; set; }
    public required ICollection<InspectionMethodDto> InspectionMethods { get; set; }
    public ICollection<EquipmentAttachmentDto> Attachments { get; set; } = new List<EquipmentAttachmentDto>();
    public int SecurityLevelId {get; set;}
    public required SecurityLevelDto? SecurityLevel {get; set;}

    public static EquipmentDto FromEquipment(Equipment equipment)
    {
        return new EquipmentDto
        {
            Id = equipment.Id,
            Name = equipment.Name,
            Model = equipment.Model,
            SerialNumber = equipment.SerialNumber,
            Manufacturer = equipment.Manufacturer,
            Category = equipment.Category,
            Quantity = equipment.Quantity,
            ExecutorId = equipment.ExecutorId,
            Executor = equipment.Executor != null
                ? UserDto.FromUser(equipment.Executor)
                : null,
            SecurityLevelId = equipment.SecurityLevelId,
            SecurityLevel = equipment.SecurityLevel != null
                ? SecurityLevelDto.FromSecurityLevel(equipment.SecurityLevel)
            : null,
            SZZ = equipment.SZZ,
            ParentId = equipment.ParentId,
            Description = equipment.Description,
            CreatedAt = equipment.CreatedAt.ToLocalTime(),
            UpdatedAt = equipment.UpdatedAt.ToLocalTime(),
            Components = equipment.Components?.Select(FromEquipment)
                .ToList(),
            Attachments = equipment.Attachments?
                              .Select(EquipmentAttachmentDto.FromEquipmentAttachment)
                              .ToList() ??
                          new List<EquipmentAttachmentDto>(),
            InspectionMethods = equipment.EquipmentInspectionMethods?
                .Select(eim => new InspectionMethodDto
                {
                    Id = eim.InspectionMethod.Id,
                    Code = eim.InspectionMethod.Code,
                    Name = eim.InspectionMethod.Name,
                    Description = eim.InspectionMethod.Description,
                    IsAlive = eim.InspectionMethod.IsAlive
                })
                .ToList(),

        };
    }
}

public class CreateEquipmentDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Model is required")]
    [StringLength(200, ErrorMessage = "Model cannot be longer than 200 characters")]
    public required string Model { get; set; }

    [Required(ErrorMessage = "SerialNumber is required")]
    [StringLength(100, ErrorMessage = "SerialNumber cannot be longer than 100 characters")]
    public required string SerialNumber { get; set; }

    [Required(ErrorMessage = "Manufacturer is required")]
    [StringLength(50, ErrorMessage = "Manufacturer cannot be longer than 50 characters")]
    public required string Manufacturer { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Category must be greater than 0")]
    public required int Category { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
    
    [Required(ErrorMessage = "ExecutorId is required")]
    public Guid ExecutorId { get; set; }
    
    [Required(ErrorMessage = "SecurityLevelId is required")]
    public int SecurityLevelId { get; set; }

    [StringLength(50, ErrorMessage = "SZZ cannot be longer than 50 characters")]
    public string? SZZ { get; set; }

    [StringLength(500, ErrorMessage = "Reasons cannot be longer than 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "At least one inspection method is required")]
    public required List<InitialInspectionMethodAssignmentDto> Methods { get; set; }
    

    public ICollection<CreateEquipmentDto>? Components { get; set; }
}

public class UpdateEquipmentDto : CreateEquipmentDto;

public class CreateEquipmentBulkDto
{
    [Required(ErrorMessage = "Equipment collection is required")]
    [MinLength(1, ErrorMessage = "At least one equipment item is required")]
    public required ICollection<CreateEquipmentDto> Equipment { get; set; }
}

public class UpdateEquipmentBulkItemDto : CreateEquipmentDto
{
    [Required(ErrorMessage = "Id is required")]
    public required Guid Id { get; set; }
}

public class UpdateEquipmentBulkDto
{
    [Required(ErrorMessage = "Equipment collection is required")]
    [MinLength(1, ErrorMessage = "At least one equipment item is required")]
    public required ICollection<UpdateEquipmentBulkItemDto> Equipment { get; set; }
}

public class DeleteEquipmentBulkResult
{
    public required ICollection<EquipmentDto> DeletedEquipment { get; set; }
    public required ICollection<Guid> FailedEquipmentIds { get; set; }
    public required ICollection<string> FailureReasons { get; set; }
}

public class UpdateEquipmentBulkResult
{
    public required ICollection<EquipmentDto> UpdatedEquipment { get; set; }
    public required ICollection<Guid> FailedEquipmentIds { get; set; }
    public required ICollection<string> FailureReasons { get; set; }
}

public class InitialInspectionMethodAssignmentDto
{
    [Required(ErrorMessage = "InspectionMethodId is required")]
    public required int InspectionMethodId { get; set; }
}

public class InitialSecurityLevelAssignmentDto
{
    [Required(ErrorMessage = "SecurityLevelId is required")]
    public required int SecurityLevelId { get; set; }
}