using AuthApp.application.DTOs;

namespace AuthApp.application.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<List<CustomerDto>> GetAllAsync();
    Task<CustomerDto> CreateAsync(CreateCustomerDto customerDto);
    Task<CreateCustomerBulkResultDto> CreateBulkAsync(CreateCustomerBulkDto customersDto);
    Task<CustomerDto?> UpdateAsync(UpdateCustomerDto updateCustomer);
    Task<List<CustomerDto>> UpdateBulkAsync(UpdateCustomerBulkDto customersDto);
    Task<CustomerDto?> DeleteAsync(DeleteCustomerDto dto);
    Task<List<CustomerDto>> DeleteBulkAsync(DeleteCustomerBulkDto customersDto);
}