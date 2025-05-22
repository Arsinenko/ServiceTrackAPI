using AuthApp.application.DTOs;
using AuthApp.application.Exceptions;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;

namespace AuthApp.application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return null;
        return CustomerDto.FromCustomer(customer);
    }

    public async Task<List<CustomerDto>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(CustomerDto.FromCustomer).ToList();
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto customerDto)
    {
        var existCustomer = await _customerRepository.GetByNameAsync(customerDto.Name);
        if (existCustomer != null)
            throw new CustomerAlreadyExistsException($"Customer with name {customerDto.Name} already exists.");
        var customer = new Customer
        {
            Name = customerDto.Name,
        };
        var result =  await _customerRepository.CreateAsync(customer);
        return CustomerDto.FromCustomer(result);
    }

    public async Task<CreateCustomerBulkResultDto> CreateBulkAsync(CreateCustomerBulkDto customers)
    {
        var createdCustomers = new List<Customer>();
        var failedCustomers = new List<CreateCustomerDto>();
        var failureReasons = new List<string>();
        var seenNames = new HashSet<string>();
        var duplicateNames = new HashSet<string>();

        foreach (var customerDto in customers.Customers)
        {
            if (!seenNames.Add(customerDto.Name))
            {
                duplicateNames.Add(customerDto.Name);
            }
        }

        foreach (var customerDto in customers.Customers)
        {
            if (duplicateNames.Contains(customerDto.Name))
            {
                failedCustomers.Add(customerDto);
                failureReasons.Add($"Customer name {customerDto.Name} duplicate in the batch");
            }
        }
        var existingCustomers = await _customerRepository.GetAllAsync();
        var existingNames = existingCustomers.Select(customer => customer.Name);
        foreach (var customerDto in customers.Customers)
        {
            if (duplicateNames.Contains(customerDto.Name))
                continue;
            if (existingNames.Contains(customerDto.Name))
            {
                failedCustomers.Add(customerDto);
                failureReasons.Add($"Customer name {customerDto.Name} already exists.");
                continue;
            }

            var customer = new Customer
            {
                Name = customerDto.Name,
            };
            createdCustomers.Add(customer);
        }

        if (createdCustomers.Any())
        {
            await _customerRepository.CreateBulkAsync(createdCustomers);
        }

        return new CreateCustomerBulkResultDto
        {
            CreatedCustomers = createdCustomers.Select(CustomerDto.FromCustomer).ToList(),
            FiledCustomers = failedCustomers,
            FailureReasons = failureReasons
        };
    }

    public async Task<CustomerDto?> UpdateAsync(UpdateCustomerDto updateCustomer)
    {
        var customer = await _customerRepository.GetByIdAsync(updateCustomer.Id);
        if (customer == null)
            throw new CustomerNotFoundException($"Customer with id {updateCustomer.Id} does not exist.");
        if (customer.Name != updateCustomer.Name)
        {
            var existingCustomer = await _customerRepository.GetByNameAsync(updateCustomer.Name);
            if (existingCustomer != null)
            {
                throw new CustomerAlreadyExistsException($"Customer with name {updateCustomer.Name} already exists.");
            }
        }
        customer.Name = updateCustomer.Name;
        var result = await _customerRepository.UpdateAsync(customer);
        return CustomerDto.FromCustomer(result);
    }

    public async Task<List<CustomerDto>> UpdateBulkAsync(UpdateCustomerBulkDto updateCustomers)
    {
        var customers = new List<Customer>();
        foreach (var customerDto in updateCustomers.Customers)
        {
            var customer = new Customer
            {
                Name = customerDto.Name
            };
            customers.Add(customer);
        }
        var result = await _customerRepository.UpdateBulkAsync(customers);
        return customers.Select(CustomerDto.FromCustomer).ToList();
    }

    public async Task<CustomerDto?> DeleteAsync(DeleteCustomerDto dto)
    {
        var customer = _customerRepository.GetByIdAsync(dto.Id);
        if (customer == null)
            throw new CustomerNotFoundException($"Customer with id {dto.Id} does not exist.");
        var result = await _customerRepository.DeleteAsync(dto.Id);
        
        return CustomerDto.FromCustomer(result);
    }

    public async Task<List<CustomerDto>> DeleteBulkAsync(DeleteCustomerBulkDto customersDto)
    {
        var ids = customersDto.Customers.Select(customer => customer.Id);
        var customers = await _customerRepository.DeleteBulkAsync(ids);
        return customers.Select(CustomerDto.FromCustomer).ToList();
    }
}