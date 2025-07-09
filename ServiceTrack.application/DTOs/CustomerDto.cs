using AuthApp.domain.Entities;

namespace AuthApp.application.DTOs;

public class CustomerDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static CustomerDto FromCustomer(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}

public class CreateCustomerDto
{
    public required string Name { get; set; }
}

public class CreateCustomerBulkDto
{
    public required ICollection<CreateCustomerDto> Customers { get; set; }
}

public class CreateCustomerBulkResultDto
{
    public required ICollection<CustomerDto> CreatedCustomers { get; set; }
    public required ICollection<CreateCustomerDto> FiledCustomers { get; set; }
    public required ICollection<string> FailureReasons { get; set; }
}

public class UpdateCustomerDto
{
    public required int Id { get; set; }
    public string? Name { get; set; }
}

public class UpdateCustomerBulkDto
{
    public required ICollection<UpdateCustomerDto> Customers { get; set; }
}

public class DeleteCustomerDto
{
    public required int Id { get; set; }
}

public class DeleteCustomerBulkDto
{
    public required ICollection<DeleteCustomerDto> Customers { get; set; }
}