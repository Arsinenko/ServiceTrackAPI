namespace AuthApp.domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsAlive { get; set; } = true;
    
    // Navigation property
    public Guid RoleId { get; set; }
    public Role Role { get; set; }
}