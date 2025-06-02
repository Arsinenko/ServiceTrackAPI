namespace AuthApp.domain.Entities;

public class EquipmentInspectionMethod
{
    public int Id { get; set; }
    public required Guid EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }
    public required int InspectionMethodId { get; set; }
    
    public InspectionMethod? InspectionMethod { get; set; }
}