using AuthApp.application.DTOs;

namespace AuthApp.application.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<List<CustomerDto>> GetAllAsync();
    Task<CustomerDto> CreateAsync(CreateCustomerDto customer);
    Task<List<CustomerDto>> CreateBulkAsync(CreateCustomerBulkDto customers);
    Task<CustomerDto> UpdateAsync(UpdateCustomerDto customer);
    Task<List<CustomerDto>> UpdateBulkAsync(UpdateCustomerBulkDto customers);
    Task<CustomerDto> DeleteAsync(DeleteCustomerDto dto);
    Task<List<CustomerDto>> DeleteBulkAsync(DeleteCustomerBulkDto customers);
}