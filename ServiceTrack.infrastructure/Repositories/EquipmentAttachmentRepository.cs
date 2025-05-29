using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.infrastructure.Repositories;

public class EquipmentAttachmentRepository : IEquipmentAttachmentRepository
{
    private readonly ApplicationDbContext _context;

    public EquipmentAttachmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<EquipmentAttachment?> GetByIdAsync(int id)
    {
        var attachment = await _context.EquipmentAttachments.FirstOrDefaultAsync(a  => a.Id == id);
        if (attachment == null)
            return null;
        return attachment;
    }

    public async Task<List<EquipmentAttachment>> GetByEquipmentIdAsync(Guid equipmentId)
    {
        return await _context.EquipmentAttachments
            .Where(a => a.EquipmentId == equipmentId)
            .ToListAsync();
    }

    public async Task<EquipmentAttachment> CreateAsync(EquipmentAttachment equipmentAttachment)
    {
        _context.EquipmentAttachments.Add(equipmentAttachment);
        await _context.SaveChangesAsync();
        return equipmentAttachment;
    }

    public async Task<List<EquipmentAttachment>> CreateBulkAsync(List<EquipmentAttachment> equipmentAttachment)
    {
        foreach (var attachment in equipmentAttachment)
        {
            attachment.UploadDate = DateTime.Now;
        }
        await _context.EquipmentAttachments.BulkInsertAsync(equipmentAttachment);
        return equipmentAttachment;
    }

    public async Task DeleteAsync(EquipmentAttachment equipmentAttachment)
    {
        _context.EquipmentAttachments.Remove(equipmentAttachment);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteBulkAsync(List<EquipmentAttachment> equipmentAttachments)
    {
        await _context.EquipmentAttachments.BulkDeleteAsync(equipmentAttachments);
    }
}