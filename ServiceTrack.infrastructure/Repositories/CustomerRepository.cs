using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using AuthApp.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Customer?> GetByIdAsync(int id)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null)
        {
            return null;
        }

        return customer;
    }

    public async Task<IEnumerable<Customer>> GetByIdsAsync(List<int> ids)
    {
        return await _context.Customers.Where(c => ids.Contains(c.Id)).ToListAsync(); 
    }

    public async Task<Customer?> GetByNameAsync(string name)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Name == name);
        if (customer == null)
            return null;
        return customer;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        var customers = await _context.Customers.ToListAsync();
        return customers;
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        customer.CreatedAt = DateTime.UtcNow; 
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<IEnumerable<Customer>> CreateBulkAsync(IEnumerable<Customer> customers)
    {
        var customerList = customers.ToList();
        foreach (var customer in customerList)
        {
            customer.CreatedAt = DateTime.UtcNow;
        }
        await _context.AddRangeAsync(customerList);
        await _context.SaveChangesAsync();
        return customerList;
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<IEnumerable<Customer>> UpdateBulkAsync(IEnumerable<Customer> customers)
    {
        var customerList = customers.ToList();
        foreach (var customer in customerList)
        {
            customer.UpdatedAt = DateTime.UtcNow;
        }

        _context.UpdateRange(customerList);
        await _context.SaveChangesAsync();
        return customerList;
    }

    public async Task<Customer?> DeleteAsync(int id)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null)
        {
            return null;
        }
        _context.Remove(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<IEnumerable<Customer>> DeleteBulkAsync(IEnumerable<int> ids)
    {
        var customers = await _context.Customers
            .Where(c => ids.Contains(c.Id))
            .ToListAsync();
            
        _context.Customers.RemoveRange(customers);
        await _context.SaveChangesAsync();
        return customers;
    }
}