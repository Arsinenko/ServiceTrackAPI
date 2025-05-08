using AuthApp.application.DTOs;

namespace AuthApp.application.Interfaces;

public interface IServiceRequestService
{
    Task<ServiceRequestDto?> GetByIdAsync(int id);
    Task<IEnumerable<ServiceRequestDto>> GetAllAsync();
    Task<IEnumerable<ServiceRequestDto>> GetByUserIdAsync(Guid userId);
    Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto createDto);
    Task<List<ServiceRequestDto>> CreateBulkAsync(CreateServiceRequestBulkDto createDto);
    Task<ServiceRequestDto?> UpdateAsync(int id, UpdateServiceRequestDto updateDto);
    Task<bool> DeleteAsync(int id);
    Task<ServiceRequestDto?> AssignUserAsync(int requestId, Guid userId, bool isPrimary = false);
    Task<ServiceRequestDto?> UnassignUserAsync(int requestId, Guid userId);
    Task<ServiceRequestDto?> AssignEquipmentAsync(int requestId, Guid equipmentId, string? notes = null);
    Task<ServiceRequestDto?> UnassignEquipmentAsync(int requestId, Guid equipmentId);
} 