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

        var dtoProperties = typeof(UpdateCustomerDto).GetProperties().Where(p => p.Name != nameof(UpdateCustomerDto.Id));
        foreach (var dtoProperty in dtoProperties)
        {
            var value = dtoProperty.GetValue(updateCustomer);
            if (value != null)
            {
                var entityProperty = customer.GetType().GetProperty(dtoProperty.Name);
                if (entityProperty != null && entityProperty.CanWrite)
                {
                    entityProperty.SetValue(customer, value);
                }
            }
        }
        var result = await _customerRepository.UpdateAsync(customer);
        return CustomerDto.FromCustomer(result);
    }

    public async Task<List<CustomerDto>> UpdateBulkAsync(UpdateCustomerBulkDto updateCustomers)
    {
        var customers = new List<Customer>();
        var updatedCustomers = new List<CustomerDto>();
        var failureReasons = new List<string>();

        foreach (var customerDto in updateCustomers.Customers)
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(customerDto.Id);
            if (existingCustomer == null)
            {
                failureReasons.Add($"Customer with id {customerDto.Id} does not exist.");
                continue;
            }

            if (existingCustomer.Name != customerDto.Name)
            {
                var customerWithSameName = await _customerRepository.GetByNameAsync(customerDto.Name);
                if (customerWithSameName != null)
                {
                    failureReasons.Add($"Customer with name {customerDto.Name} already exists.");
                    continue;
                }
            }

            existingCustomer.Name = customerDto.Name;
            customers.Add(existingCustomer);
        }

        if (customers.Any())
        {
            var result = await _customerRepository.UpdateBulkAsync(customers);
            updatedCustomers.AddRange(result.Select(CustomerDto.FromCustomer));
        }

        return updatedCustomers;
    }

    public async Task<CustomerDto?> DeleteAsync(DeleteCustomerDto dto)
    {
        var customer = await _customerRepository.GetByIdAsync(dto.Id);
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