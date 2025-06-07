using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ServiceTrack.application.Interfaces;

namespace AuthApp.infrastructure.Repositories;

public class SecurityLevelRepository : ISecurityLevelRepository
{
    private readonly ApplicationDbContext _context;

    public SecurityLevelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SecurityLevel>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _context.SecurityLevels.Where(sl => ids.Contains(sl.Id)).ToListAsync(); 
    }

    public async Task<SecurityLevel?> GetByIdAsync(int id)
    {
        return await _context.SecurityLevels.FindAsync(id);
    }

    public async Task<IEnumerable<SecurityLevel>> GetAllAsync()
    {
        return await _context.SecurityLevels.ToListAsync();
    }

    public async Task<SecurityLevel> CreateAsync(SecurityLevel securityLevel)
    {
        _context.SecurityLevels.Add(securityLevel);
        await _context.SaveChangesAsync();
        return securityLevel;
    }

    public async Task<SecurityLevel> UpdateAsync(SecurityLevel securityLevel)
    {
        _context.Entry(securityLevel).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return securityLevel;
    }

    public async Task DeleteAsync(int id)
    {
        var securityLevel = await _context.SecurityLevels.FindAsync(id);
        if (securityLevel != null)
        {
            _context.SecurityLevels.Remove(securityLevel);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null)
    {
        var query = _context.SecurityLevels.Where(sl => sl.Code == code);
        if (excludeId.HasValue)
        {
            query = query.Where(sl => sl.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var query = _context.SecurityLevels.Where(sl => sl.Name == name);
        if (excludeId.HasValue)
        {
            query = query.Where(sl => sl.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }
} 