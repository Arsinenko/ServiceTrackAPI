namespace AuthApp.domain.Entities;

public class InspectionMethod
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required bool IsAlive { get; set; }
}