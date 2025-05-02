using System;
using System.Collections.Generic;

namespace AuthApp.domain.Entities;

public class EquipmentComponent
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public string SerialNumber { get; set; }
    public string Manufacturer { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public Guid EquipmentId { get; set; }
    public Equipment Equipment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Связь с родительским компонентом
    public Guid? ParentComponentId { get; set; }
    public EquipmentComponent ParentComponent { get; set; }
    
    // Коллекция дочерних компонентов
    public ICollection<EquipmentComponent> ChildComponents { get; set; }
} 