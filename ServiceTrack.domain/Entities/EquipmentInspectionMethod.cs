using AuthApp.domain.Entities;

public class EquipmentInspectionMethod
{
    public Guid EquipmentID { get; set; }
    public required Equipment Equipment { get; set; }
    public int InspectionMethodId { get; set; }
    
    public required InspectionMethod InspectionMethod { get; set; }
}