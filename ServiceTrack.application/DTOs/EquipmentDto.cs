using AuthApp.domain.Entities;

namespace AuthApp.application.DTOs;

public class EquipmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public string SerialNumber { get; set; }
    public string Manufacturer { get; set; }
    public int Quantity { get; set; }
    public Guid? ParentId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<EquipmentDto>? Components { get; set; }

    public static EquipmentDto FromEquipment(Equipment equipment)
    {
        return new EquipmentDto
        {
            Id = equipment.Id,
            Name = equipment.Name,
            Model = equipment.Model,
            SerialNumber = equipment.SerialNumber,
            Manufacturer = equipment.Manufacturer,
            Quantity = equipment.Quantity,
            ParentId = equipment.ParentId,
            Description = equipment.Description,
            CreatedAt = equipment.CreatedAt.ToLocalTime(),
            UpdatedAt = equipment.UpdatedAt.ToLocalTime(),
            Components = equipment.Components?.Select(c => FromEquipment(c)).ToList()
        };
    }
}

public class CreateEquipmentDto
{
    public required string Name { get; set; }
    public required string Model { get; set; }
    public required string SerialNumber { get; set; }
    public required string Manufacturer { get; set; }
    public int Quantity { get; set; }
    public Guid? ParentId { get; set; }
    public string? Description { get; set; }
    public ICollection<CreateEquipmentDto>? Components { get; set; }
}

public class UpdateEquipmentDto : CreateEquipmentDto;

public class CreateEquipmentBulkDto
{
    public required ICollection<CreateEquipmentDto> Equipment { get; set; }
}

public class UpdateEquipmentBulkItemDto : CreateEquipmentDto
{
    public required Guid Id { get; set; }
}

public class UpdateEquipmentBulkDto
{
    public required ICollection<UpdateEquipmentBulkItemDto> Equipment { get; set; }
}

public class DeleteEquipmentBulkResult
{
    public required ICollection<EquipmentDto> DeletedEquipment { get; set; }
    public required ICollection<Guid> FailedEquipmentIds { get; set; }
    public required ICollection<string> FailureReasons { get; set; }
}