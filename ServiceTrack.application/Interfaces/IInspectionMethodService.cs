using AuthApp.application.DTOs;
using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IInspectionMethodService
{
    Task<IEnumerable<InspectionMethodDto>> GetAllAsync();
    Task<InspectionMethodDto?> GetByIdAsync(int id);
    Task<InspectionMethodDto?> GetByNameAsync(string code);
    Task<InspectionMethodDto> CreateAsync(CreateInspectionMethodItemDto inspectionMethod);
    Task<IEnumerable<InspectionMethodDto>> CreateBulkAsync(CreateInspectionMethodsDto inspectionMethods);
    Task<InspectionMethodDto> UpdateAsync(UpdateInspectionMethodItemDto inspectionMethod);
    Task<List<InspectionMethodDto>> UpdateBulkAsync(UpdateInspectionMethodsDto inspectionMethods);
    Task<List<InspectionMethodDto>> DeleteAsync(List<InspectionMethod> inspectionMethods);
}