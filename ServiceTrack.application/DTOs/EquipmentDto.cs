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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<EquipmentComponentDto> Components { get; set; }

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
            CreatedAt = equipment.CreatedAt.ToLocalTime(),
            UpdatedAt = equipment.UpdatedAt.ToLocalTime(),
            Components = equipment.Components?.Select(c => EquipmentComponentDto.FromEquipmentComponent(c)).ToList()
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
}

public class UpdateEquipmentDto : CreateEquipmentDto;
