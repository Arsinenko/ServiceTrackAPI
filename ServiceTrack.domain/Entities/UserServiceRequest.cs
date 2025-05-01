namespace AuthApp.domain.Entities;

public class UserServiceRequest
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public int ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; }
    
    // Additional properties for the relationship
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public bool IsPrimaryAssignee { get; set; }
}