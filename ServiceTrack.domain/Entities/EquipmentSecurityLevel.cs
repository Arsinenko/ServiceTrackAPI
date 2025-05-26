using AuthApp.domain.Entities;

public class EquipmentSecurityLevel 
{
    public Guid Id { get; set; }
    public required Guid EquipmentId { get; set; }
    public required Equipment Equipment { get; set; }
    public required int SecurityLevelId { get; set; }
    public required SecurityLevel SecurityLevel { get; set; }
}