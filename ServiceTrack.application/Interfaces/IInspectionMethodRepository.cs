using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface IInspectionMethodRepository
{
    Task<List<InspectionMethod>> GetAllAsync();
    Task<InspectionMethod?> GetByIdAsync(int id);
    Task<IEnumerable<InspectionMethod>> GetByIdsAsync(IEnumerable<int> ids);
    Task<InspectionMethod?> GetByNameAsync(string code);
    Task<InspectionMethod> CreateAsync(InspectionMethod inspectionMethod);
    Task<List<InspectionMethod>> CreateBulkAsync(List<InspectionMethod> inspectionMethods);
    Task<List<InspectionMethod>> UpdateBulkAsync(List<InspectionMethod> inspectionMethods);
    
    Task<List<InspectionMethod>> DeleteBulkAsync(List<InspectionMethod> inspectionMethods);
    
}