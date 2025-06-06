using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace AuthApp.application.Services;

public class EquipmentAttachmentService : IEquipmentAttachmentService
{
    private readonly IEquipmentAttachmentRepository _attachmentRepository;
    private readonly string _uploadDirectory;

    public EquipmentAttachmentService(
        IEquipmentAttachmentRepository attachmentRepository,
        IHostEnvironment environment)
    {
        _attachmentRepository = attachmentRepository;
        
        // Use test directory if running in test environment
        if (environment.EnvironmentName == "Test")
        {
            _uploadDirectory = Path.Combine(Path.GetTempPath(), "equipment_attachments_test");
        }
        else
        {
            _uploadDirectory = Path.Combine(environment.ContentRootPath, "wwwroot", "uploads", "equipment");
        }
        
        // Create directory if it doesn't exist
        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }
    }

    public async Task<EquipmentAttachment> SaveAttachmentAsync(IFormFile file, Guid equipmentId, string? description = null)
    {
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(_uploadDirectory, fileName);

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var attachment = new EquipmentAttachment
        {
            Id = 0, // Will be set by the database
            EquipmentId = equipmentId,
            FileName = file.FileName,
            FileSize = file.Length,
            FilePath = fileName,
            FileType = file.ContentType,
            UploadDate = DateTime.UtcNow,
            Equipment = null // Will be set by the database
        };

        return await _attachmentRepository.CreateAsync(attachment);
    }

    public async Task<List<EquipmentAttachment>> SaveAttachmentsAsync(List<IFormFile> files, Guid equipmentId, string? description = null)
    {
        var attachments = new List<EquipmentAttachment>();

        foreach (var file in files)
        {
            var attachment = await SaveAttachmentAsync(file, equipmentId, description);
            attachments.Add(attachment);
        }

        return attachments;
    }

    public async Task DeleteAttachmentAsync(int attachmentId)
    {
        var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
        if (attachment != null)
        {
            var filePath = Path.Combine(_uploadDirectory, attachment.FilePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            await _attachmentRepository.DeleteAsync(attachment);
        }
    }

    public async Task DeleteAttachmentsAsync(List<int> attachmentIds)
    {
        var attachments = new List<EquipmentAttachment>();
        foreach (var id in attachmentIds)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(id);
            if (attachment != null)
            {
                var filePath = Path.Combine(_uploadDirectory, attachment.FilePath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                attachments.Add(attachment);
            }
        }
        await _attachmentRepository.DeleteBulkAsync(attachments);
    }

    public async Task<EquipmentAttachment?> GetByIdAsync(int id)
    {
        return await _attachmentRepository.GetByIdAsync(id);
    }

    public async Task<List<EquipmentAttachment>> GetAttachmentsByEquipmentIdAsync(Guid equipmentId)
    {
        return await _attachmentRepository.GetByEquipmentIdAsync(equipmentId);
    }

    public async Task<(byte[] FileContent, string FileName, string ContentType)> GetAttachmentFileAsync(int attachmentId)
    {
        var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
        if (attachment == null)
        {
            throw new FileNotFoundException($"Attachment with ID {attachmentId} not found");
        }

        var filePath = Path.Combine(_uploadDirectory, attachment.FilePath);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found at path: {filePath}");
        }

        var fileContent = await File.ReadAllBytesAsync(filePath);
        return (fileContent, attachment.FileName, attachment.FileType ?? "application/octet-stream");
    }
} 