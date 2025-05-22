using AuthApp.domain.Entities;

namespace AuthApp.application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByNameAsync(string name);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer> CreateAsync(Customer customer);
    Task<IEnumerable<Customer>> CreateBulkAsync(IEnumerable<Customer> customers);
    Task<Customer> UpdateAsync(Customer customer);
    Task<IEnumerable<Customer>> UpdateBulkAsync(IEnumerable<Customer> customers);
    
    Task <Customer?> DeleteAsync(int id);
    Task<IEnumerable<Customer>> DeleteBulkAsync(IEnumerable<int> ids);
}