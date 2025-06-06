using AuthApp.domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AuthApp.application.Interfaces;

public interface IEquipmentAttachmentService
{
    Task<EquipmentAttachment> SaveAttachmentAsync(IFormFile file, Guid equipmentId, string? description = null);
    Task<List<EquipmentAttachment>> SaveAttachmentsAsync(List<IFormFile> files, Guid equipmentId, string? description = null);
    Task DeleteAttachmentAsync(int attachmentId);
    Task DeleteAttachmentsAsync(List<int> attachmentIds);
    Task<EquipmentAttachment?> GetByIdAsync(int id);
    Task<List<EquipmentAttachment>> GetAttachmentsByEquipmentIdAsync(Guid equipmentId);
    Task<(byte[] FileContent, string FileName, string ContentType)> GetAttachmentFileAsync(int attachmentId);
}