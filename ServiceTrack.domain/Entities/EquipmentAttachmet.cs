namespace AuthApp.domain.Entities;

public class EquipmentAttachment
{
    public required int Id { get; set; }
    public required Guid EquipmentId { get; set; }
    public required string FileName { get; set; }
    public required double FileSize { get; set; }
    public required string FilePath { get; set; }
    public string? FileType { get; set; }  // Тип файла (изображение/текст)
    public required DateTime UploadDate { get; set; }
    
    public required Equipment Equipment { get; set; }
}