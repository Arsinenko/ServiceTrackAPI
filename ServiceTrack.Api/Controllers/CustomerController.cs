using AuthApp.application.DTOs;
using AuthApp.application.Exceptions;
using AuthApp.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Api.Controllers;

/// <summary>
/// Контроллер для управления заказчиками
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Получает заказчика по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор заказчика</param>
    /// <returns>Данные заказчика</returns>
    /// <response code="200">Возвращает данные заказчика</response>
    /// <response code="404">Заказчик не найден</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
            return NotFound();
        return Ok(customer);
    }

    /// <summary>
    /// Получает список всех заказчиков
    /// </summary>
    /// <returns>Список заказчиков</returns>
    /// <response code="200">Возвращает список заказчиков</response>
    [HttpGet]
    public async Task<ActionResult<List<CustomerDto>>> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    /// <summary>
    /// Создает нового заказчика
    /// </summary>
    /// <param name="customerDto">Данные для создания заказчика</param>
    /// <returns>Созданный заказчик</returns>
    /// <response code="201">Заказчик успешно создан</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="409">Заказчик с таким именем уже существует</response>
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto customerDto)
    {
        try
        {
            var customer = await _customerService.CreateAsync(customerDto);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }
        catch (CustomerAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Создает несколько заказчиков
    /// </summary>
    /// <param name="customers">Данные для создания заказчиков</param>
    /// <returns>Результат создания заказчиков</returns>
    /// <response code="200">Операция завершена</response>
    /// <response code="400">Некорректные данные</response>
    [HttpPost("bulk")]
    public async Task<ActionResult<CreateCustomerBulkResultDto>> CreateBulk([FromBody] CreateCustomerBulkDto customers)
    {
        var result = await _customerService.CreateBulkAsync(customers);
        return Ok(result);
    }

    /// <summary>
    /// Обновляет данные заказчика (можно передавать только изменяемые поля)
    /// </summary>
    /// <param name="customerDto">Данные для обновления заказчика (только изменяемые поля)</param>
    /// <returns>Обновленный заказчик</returns>
    /// <response code="200">Заказчик успешно обновлен</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="404">Заказчик не найден</response>
    /// <response code="409">Заказчик с таким именем уже существует</response>
    [HttpPut]
    public async Task<ActionResult<CustomerDto>> Update([FromBody] UpdateCustomerDto customerDto)
    {
        try
        {
            var customer = await _customerService.UpdateAsync(customerDto);
            return Ok(customer);
        }
        catch (CustomerNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (CustomerAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Обновляет список заказчиков
    /// </summary>
    /// <param name="customers">Данные для обновления заказчиков</param>
    /// <returns>Список обновленных заказчиков</returns>
    /// <response code="200">Заказчики успешно обновлены</response>
    /// <response code="400">Некорректные данные</response>
    [HttpPut("bulk")]
    public async Task<ActionResult<List<CustomerDto>>> UpdateBulk([FromBody] UpdateCustomerBulkDto customers)
    {
        var updatedCustomers = await _customerService.UpdateBulkAsync(customers);
        return Ok(updatedCustomers);
    }

    /// <summary>
    /// Удаляет заказчика
    /// </summary>
    /// <param name="dto">Данные для удаления заказчика</param>
    /// <returns>Удаленный заказчик</returns>
    /// <response code="200">Заказчик успешно удален</response>
    /// <response code="404">Заказчик не найден</response>
    [HttpDelete]
    public async Task<ActionResult<CustomerDto>> Delete([FromBody] DeleteCustomerDto dto)
    {
        try
        {
            var customer = await _customerService.DeleteAsync(dto);
            return Ok(customer);
        }
        catch (CustomerNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Удаляет список заказчиков
    /// </summary>
    /// <param name="customers">Данные для удаления заказчиков</param>
    /// <returns>Список удаленных заказчиков</returns>
    /// <response code="200">Заказчики успешно удалены</response>
    /// <response code="400">Некорректные данные</response>
    [HttpDelete("bulk")]
    public async Task<ActionResult<List<CustomerDto>>> DeleteBulk([FromBody] DeleteCustomerBulkDto customers)
    {
        var deletedCustomers = await _customerService.DeleteBulkAsync(customers);
        return Ok(deletedCustomers);
    }
} 