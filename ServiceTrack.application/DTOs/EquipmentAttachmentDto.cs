using AuthApp.application.DTOs;
using AuthApp.domain.Entities;

public class EquipmentAttachmentDto 
{
    public required int Id { get; set; }
    public required Guid EquipmentID { get; set; }
    public required string FileName { get; set; }
    public required double FileSize { get; set; }
    public required string FilePath { get; set; }
    public string? FileType { get; set; }
    public string? Description { get; set; }
    public required DateTime UploadDate { get; set; }
    
    public static EquipmentAttachmentDto FromEquipmentAttachment(EquipmentAttachment equipmentAttachment)
    {
        return new EquipmentAttachmentDto
        {
            Id = equipmentAttachment.Id,
            EquipmentID = equipmentAttachment.EquipmentID,
            FileName = equipmentAttachment.FileName,
            FilePath = equipmentAttachment.FilePath,
            FileSize = equipmentAttachment.FileSize,
            FileType = equipmentAttachment.FileType,
            Description = equipmentAttachment.Description,
            UploadDate = equipmentAttachment.UploadDate
        };
    }
}