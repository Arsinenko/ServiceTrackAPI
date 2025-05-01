namespace AuthApp.domain.Entities;

public class ServiceRequestEquipment
{
    public int ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; }
    
    public Guid EquipmentId { get; set; }
    public Equipment Equipment { get; set; }
    
    // Additional properties for the relationship
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}