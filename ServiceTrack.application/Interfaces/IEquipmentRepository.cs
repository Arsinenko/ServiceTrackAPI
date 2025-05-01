using AuthApp.application.DTOs;
using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IEquipmentRepository
{
    Task<Equipment?> GetByIdAsync(Guid id);
    Task<Equipment?> GetByNameAsync(string name);
    Task<IEnumerable<Equipment>> GetAllAsync();
    Task<Guid> CreateAsync(Equipment equipment);
    Task<Guid?> UpdateAsync(Equipment equipment);
    Task DeleteAsync(Guid id);
}