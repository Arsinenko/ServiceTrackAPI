namespace AuthApp.domain.Entities;

public class ServiceRequest
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public string Customer { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // Many-to-many relationship with User
    public ICollection<UserServiceRequest> UserServiceRequests { get; set; }
    
    // Many-to-many relationship with Equipment
    public ICollection<ServiceRequestEquipment> ServiceRequestEquipments { get; set; }
}