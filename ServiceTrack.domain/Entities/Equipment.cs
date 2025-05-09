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
    public Guid? ParentId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<Equipment>? Components { get; set; } 
    
    // Many-to-many relationship with ServiceRequest
    public ICollection<ServiceRequestEquipment> ServiceRequestEquipments { get; set; }
}   