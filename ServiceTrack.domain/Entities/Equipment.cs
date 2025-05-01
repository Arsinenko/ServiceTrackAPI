using System;
using System.Collections.Generic;

namespace AuthApp.domain.Entities;

public class Equipment
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public string SerialNumber { get; set; }
    public string Manufacturer { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Навигационное свойство для компонентов
    public ICollection<EquipmentComponent> Components { get; set; }
    
    // Many-to-many relationship with ServiceRequest
    public ICollection<ServiceRequestEquipment> ServiceRequestEquipments { get; set; }
}   