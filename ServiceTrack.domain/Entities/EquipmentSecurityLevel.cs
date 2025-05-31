namespace AuthApp.domain.Entities;

public class EquipmentSecurityLevel 
{
    public int Id { get; set; }
    public Guid EquipmentId { get; set; }
    public required Equipment Equipment { get; set; }
    public int SecurityLevelId { get; set; }
    public required SecurityLevel SecurityLevel { get; set; }
}