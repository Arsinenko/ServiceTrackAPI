using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CustomerRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Customer?> GetByIdAsync(int id)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null)
        {
            return null;
        }

        return customer;
    }

    public async Task<IEnumerable<Customer>> GetByIdsAsync(List<int> ids)
    {
        return await _dbContext.Customers.Where(c => ids.Contains(c.Id)).ToListAsync(); 
    }

    public async Task<Customer?> GetByNameAsync(string name)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Name == name);
        if (customer == null)
            return null;
        return customer;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        var customers = await _dbContext.Customers.ToListAsync();
        return customers;
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        customer.CreatedAt = DateTime.UtcNow; 
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();
        return customer;
    }

    public async Task<IEnumerable<Customer>> CreateBulkAsync(IEnumerable<Customer> customers)
    {
        var customerList = customers.ToList();
        foreach (var customer in customerList)
        {
            customer.CreatedAt = DateTime.UtcNow;
        }
        await _dbContext.AddRangeAsync(customerList);
        await _dbContext.SaveChangesAsync();
        return customerList;
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        _dbContext.Customers.Update(customer);
        await _dbContext.SaveChangesAsync();
        return customer;
    }

    public async Task<IEnumerable<Customer>> UpdateBulkAsync(IEnumerable<Customer> customers)
    {
        var customerList = customers.ToList();
        foreach (var customer in customerList)
        {
            customer.UpdatedAt = DateTime.UtcNow;
            _dbContext.Customers.Update(customer);
        }
        await _dbContext.SaveChangesAsync();
        return customerList;
    }

    public async Task<Customer?> DeleteAsync(int id)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null)
        {
            return null;
        }
        _dbContext.Remove(customer);
        await _dbContext.SaveChangesAsync();
        return customer;
    }

    public async Task<IEnumerable<Customer>> DeleteBulkAsync(IEnumerable<int> ids)
    {
        var customers = await _dbContext.Customers
            .Where(c => ids.Contains(c.Id))
            .ToListAsync();
            
        _dbContext.Customers.RemoveRange(customers);
        await _dbContext.SaveChangesAsync();
        return customers;
    }
}