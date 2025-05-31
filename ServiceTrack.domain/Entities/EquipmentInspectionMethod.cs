namespace AuthApp.domain.Entities;

public class EquipmentInspectionMethod
{
    public int Id { get; set; }
    public Guid EquipmentId { get; set; }
    public required Equipment Equipment { get; set; }
    public int InspectionMethodId { get; set; }
    
    public required InspectionMethod InspectionMethod { get; set; }
}