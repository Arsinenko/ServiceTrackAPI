using System;
using System.Collections.Generic;

namespace AuthApp.domain.Entities;

public class Equipment
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Model { get; set; }
    public required string SerialNumber { get; set; }
    public required string Manufacturer { get; set; }
    public required int Category { get; set; }
    public int Quantity { get; set; }
    public Guid? ExecutorId { get; set; }  // Foreign key for the executor
    public User? Executor { get; set; }    // Navigation property to User
    // ReSharper disable once InconsistentNaming
    public string? SZZ { get; set; }      // СЗЗ
    public Guid? ParentId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public int SecurityLevelId { get; set; }
    public SecurityLevel? SecurityLevel { get; set; }
    
    public required ICollection<EquipmentInspectionMethod> EquipmentInspectionMethods { get; set; }
    
    public ICollection<EquipmentAttachment> Attachments { get; set; } = new List<EquipmentAttachment>();
    
    public ICollection<Equipment>? Components { get; set; } 
    
    // Many-to-many relationship with ServiceRequest
    public ICollection<ServiceRequestEquipment>? ServiceRequestEquipments { get; set; }
}   