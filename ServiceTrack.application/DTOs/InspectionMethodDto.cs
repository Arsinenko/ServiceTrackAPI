using AuthApp.domain.Entities;

namespace AuthApp.application.DTOs;

public class InspectionMethodDto
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required bool IsAlive { get; set; }

    public static InspectionMethodDto FroMethodDto(InspectionMethod inspectionMethod)
    {
        return new InspectionMethodDto
        {
            Id = inspectionMethod.Id,
            Code = inspectionMethod.Code,
            Name = inspectionMethod.Name,
            Description = inspectionMethod.Description,
            IsAlive = inspectionMethod.IsAlive
        };
    }
}

public abstract class CreateInspectionMethodItemDto
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class CreateInspectionMethodsDto
{
    public required IEnumerable<CreateInspectionMethodItemDto> InspectionMethodItems { get; set; }
}

public class UpdateInspectionMethodItemDto : CreateInspectionMethodItemDto
{
    public int Id { get; set; }
    public bool IsAlive { get; set; }
}

public class UpdateInspectionMethodsDto
{
    public required IEnumerable<UpdateInspectionMethodItemDto> InspectionMethodItems { get; set; }
}