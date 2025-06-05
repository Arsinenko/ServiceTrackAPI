using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<IEnumerable<Role>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _context.Roles
            .Include(r => r.Users)
            .Where(r => ids.Contains(r.Id))
            .ToListAsync();
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Guid> CreateAsync(Role role)
    {
        role.CreatedAt = DateTime.UtcNow;
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role.Id;
    }

    public async Task<List<Guid>> CreateBulkAsync(IEnumerable<Role> roles)
    {
        var roleList = roles.ToList();
        foreach (var role in roleList)
        {
            role.CreatedAt = DateTime.UtcNow;
            _context.Roles.Add(role);
        }
        await _context.SaveChangesAsync();
        return roleList.Select(r => r.Id).ToList();
    }

    public async Task<Role> UpdateAsync(Role role)
    {
        role.UpdatedAt = DateTime.UtcNow;
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<List<Role>> UpdateBulkAsync(IEnumerable<Role> roles)
    {
        var roleList = roles.ToList();
        foreach (var role in roleList)
        {
            role.UpdatedAt = DateTime.UtcNow;
            _context.Roles.Update(role);
        }
        await _context.SaveChangesAsync();
        return roleList;
    }

    public async Task DeleteAsync(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role != null)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
} 