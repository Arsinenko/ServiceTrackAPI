using AuthApp.domain.Entities;

namespace AuthApp.application.DTOs;

public class EquipmentComponentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public string SerialNumber { get; set; }
    public string Manufacturer { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public Guid EquipmentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Связь с родительским компонентом
    public Guid? ParentComponentId { get; set; }
    
    // Коллекция дочерних компонентов
    public ICollection<EquipmentComponentDto> ChildComponents { get; set; }

    public static EquipmentComponentDto FromEquipmentComponent(EquipmentComponent component)
    {
        return new EquipmentComponentDto
        {
            Id = component.Id,
            Name = component.Name,
            Model = component.Model,
            SerialNumber = component.SerialNumber,
            Manufacturer = component.Manufacturer,
            Description = component.Description,
            Quantity = component.Quantity,
            EquipmentId = component.EquipmentId,
            CreatedAt = component.CreatedAt.ToLocalTime(),
            UpdatedAt = component.UpdatedAt.ToLocalTime(),
            ParentComponentId = component.ParentComponentId,
            ChildComponents = component.ChildComponents?.Select(c => FromEquipmentComponent(c)).ToList()
        };
    }
}

public class CreateEquipmentComponentDto
{
    public string Name { get; set; }
    public string Model { get; set; }
    public string SerialNumber { get; set; }
    public string Manufacturer { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public Guid EquipmentId { get; set; }
    public Guid? ParentComponentId { get; set; }
}

public class UpdateEquipmentComponentDto : CreateEquipmentComponentDto; 