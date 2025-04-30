using AuthApp.application.DTOs;
using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IEquipmentRepository
{
    Task<Equipment?> GetByIdAsync(Guid id);
    Task<Equipment?> GetByNameAsync(string name);
    Task<IEnumerable<Equipment>> GetAllAsync();
    Task<Equipment> CreateAsync(Equipment equipment);
    Task<Equipment?> UpdateAsync(Equipment equipment);
    Task DeleteAsync(Guid id);
}