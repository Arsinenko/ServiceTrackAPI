namespace AuthApp.domain.Entities;

public class ServiceRequest
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public required string RequestNumber { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    public required string Reasons { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime PlannedCompletionDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // Job type relationship
    public Guid JobTypeId { get; set; }
    public JobType JobType { get; set; }
    
    // Many-to-many relationship with User
    public ICollection<UserServiceRequest> UserServiceRequests { get; set; }
    
    // Many-to-many relationship with Equipment
    public ICollection<ServiceRequestEquipment> ServiceRequestEquipments { get; set; }
}