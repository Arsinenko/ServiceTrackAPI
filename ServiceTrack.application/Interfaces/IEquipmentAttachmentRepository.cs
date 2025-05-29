using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IEquipmentAttachmentRepository
{
    Task<EquipmentAttachment?> GetByIdAsync(int id);
    Task<List<EquipmentAttachment>> GetByEquipmentIdAsync(Guid equipmentId);
    Task<EquipmentAttachment> CreateAsync(EquipmentAttachment equipmentAttachment);
    Task<List<EquipmentAttachment>> CreateBulkAsync(List<EquipmentAttachment> equipmentAttachment);
    Task DeleteAsync(EquipmentAttachment equipmentAttachment);
    Task DeleteBulkAsync(List<EquipmentAttachment> equipmentAttachments);
}