using System.Security.Claims;
using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Api.Controllers;

/// <summary>
/// Контроллер для управления заявками на обслуживание
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ServiceRequestController : ControllerBase
{
    private readonly IServiceRequestService _serviceRequestService;

    public ServiceRequestController(IServiceRequestService serviceRequestService)
    {
        _serviceRequestService = serviceRequestService;
    }

    /// <summary>
    /// Получает список всех заявок на обслуживание
    /// </summary>
    /// <returns>Список заявок</returns>
    /// <response code="200">Возвращает список заявок</response>
    /// <response code="401">Требуется авторизация</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetAll()
    {
        var requests = await _serviceRequestService.GetAllAsync();
        return Ok(requests);
    }

    /// <summary>
    /// Получает заявку по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор заявки</param>
    /// <returns>Заявка на обслуживание</returns>
    /// <response code="200">Возвращает заявку</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="404">Заявка не найдена</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceRequestDto>> GetById(int id)
    {
        var request = await _serviceRequestService.GetByIdAsync(id);
        if (request == null)
            return NotFound();

        return Ok(request);
    }

    /// <summary>
    /// Получает список заявок, назначенных пользователю
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Список заявок</returns>
    /// <response code="200">Возвращает список заявок</response>
    /// <response code="401">Требуется авторизация</response>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetByUserId(Guid userId)
    {
        var requests = await _serviceRequestService.GetByUserIdAsync(userId);
        return Ok(requests);
    }

    /// <summary>
    /// Получает список заявок для текущего пользователя
    /// </summary>
    /// <returns>Список заявок</returns>
    /// <response code="200">Возвращает список заявок</response>
    /// <response code="401">Требуется авторизация</response>
    [Authorize]
    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetForCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }
        var requests = await _serviceRequestService.GetByUserIdAsync(userId);
        return Ok(requests);
    }

    /// <summary>
    /// Создает новую заявку на обслуживание
    /// </summary>
    /// <param name="createDto">Данные для создания заявки</param>
    /// <returns>Созданная заявка</returns>
    /// <response code="201">Заявка успешно создана</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="401">Требуется авторизация</response>
    [HttpPost]
    public async Task<ActionResult<ServiceRequestDto>> Create(CreateServiceRequestDto createDto)
    {
        var request = await _serviceRequestService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
    }
    
    /// <summary>
    /// Создает заявки на обслуживание
    /// </summary>
    /// <param name="createDto">Данные для создания заявок</param>
    /// <returns>Созданные заявки</returns>
    /// <response code="201">Заявки успешно созданы</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="401">Требуется авторизация</response>
    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> CreateBulk(CreateServiceRequestBulkDto createDto)
    {
        var requests = await _serviceRequestService.CreateBulkAsync(createDto);
        return CreatedAtAction(nameof(GetAll), requests);
    }

    /// <summary>
    /// Создает заявки на обслуживание с новым оборудованием
    /// </summary>
    /// <param name="createDto">Данные для создания заявок с новым оборудованием</param>
    /// <returns>Созданные заявки</returns>
    /// <response code="201">Заявки успешно созданы</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="401">Требуется авторизация</response>
    [HttpPost("bulk-with-equipment")]
    public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> CreateBulkWithEquipment(CreateServiceRequestWithNewEquipmentBulkDto createDto)
    {
        var requests = await _serviceRequestService.CreateBulkWithNewEquipmentAsync(createDto);
        return CreatedAtAction(nameof(GetAll), requests);
    }

    /// <summary>
    /// Обновляет существующую заявку
    /// </summary>
    /// <param name="id">Идентификатор заявки</param>
    /// <param name="updateDto">Данные для обновления</param>
    /// <returns>Обновленная заявка</returns>
    /// <response code="200">Заявка успешно обновлена</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="404">Заявка не найдена</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<ServiceRequestDto>> Update(int id, UpdateServiceRequestDto updateDto)
    {
        var request = await _serviceRequestService.UpdateAsync(id, updateDto);
        if (request == null)
            return NotFound();

        return Ok(request);
    }


    /// <summary>
    /// Обновляет несколько заявок на обслуживание
    /// </summary>
    /// <param name="updateDto">Данные для обновления заявок</param>
    /// <returns>Список обновленных заявок</returns>
    /// <response code="200">Заявки успешно обновлены</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="404">Некоторые заявки не найдены</response>
    [HttpPut("bulk")]
    public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> UpdateBulk(UpdateServiceRequestBulkDto updateDto)
    {
        var requests = await _serviceRequestService.UpdateBulkAsync(updateDto);
        return Ok(requests);
    }

    /// <summary>
    /// Удаляет заявку
    /// </summary>
    /// <param name="id">Идентификатор заявки</param>
    /// <returns>Нет содержимого</returns>
    /// <response code="204">Заявка успешно удалена</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="404">Заявка не найдена</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _serviceRequestService.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Удаляет несколько заявок на обслуживание
    /// </summary>
    /// <param name="ids">Список идентификаторов заявок для удаления</param>
    /// <returns>Результат массового удаления, содержащий информацию об успешно удаленных заявках и ошибках</returns>
    /// <response code="200">Возвращает результат операции с информацией об успешных удалениях и ошибках</response>
    /// <response code="401">Требуется авторизация</response>
    [HttpDelete("bulk")]
    public async Task<ActionResult<DeleteServiceRequestBulkResult>> DeleteBulkAsync(List<int> ids)
    {
        var result = await _serviceRequestService.DeleteBulkAsync(ids);
        return Ok(result);
    }

    /// <summary>
    /// Назначает пользователя на заявку
    /// </summary>
    /// <param name="id">Идентификатор заявки</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="isPrimary">Является ли пользователь основным исполнителем</param>
    /// <returns>Обновленная заявка</returns>
    /// <response code="200">Пользователь успешно назначен</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="404">Заявка или пользователь не найдены</response>
    [HttpPost("{id}/assign/{userId}")]
    public async Task<ActionResult<ServiceRequestDto>> AssignUser(int id, Guid userId, [FromQuery] bool isPrimary = false)
    {
        var request = await _serviceRequestService.AssignUserAsync(id, userId, isPrimary);
        if (request == null)
            return NotFound();

        return Ok(request);
    }

    /// <summary>
    /// Отменяет назначение пользователя на заявку
    /// </summary>
    /// <param name="id">Идентификатор заявки</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Обновленная заявка</returns>
    /// <response code="200">Назначение успешно отменено</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="404">Заявка не найдена</response>
    [HttpDelete("{id}/unassign/{userId}")]
    public async Task<ActionResult<ServiceRequestDto>> UnassignUser(int id, Guid userId)
    {
        var request = await _serviceRequestService.UnassignUserAsync(id, userId);
        if (request == null)
            return NotFound();

        return Ok(request);
    }

    /// <summary>
    /// Назначает оборудование на заявку
    /// </summary>
    /// <param name="id">Идентификатор заявки</param>
    /// <param name="equipmentId">Идентификатор оборудования</param>
    /// <param name="notes">Примечания к назначению</param>
    /// <returns>Обновленная заявка</returns>
    /// <response code="200">Оборудование успешно назначено</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="404">Заявка или оборудование не найдены</response>
    [HttpPost("{id}/equipment/{equipmentId}")]
    public async Task<ActionResult<ServiceRequestDto>> AssignEquipment(int id, Guid equipmentId, [FromQuery] string? notes = null)
    {
        var request = await _serviceRequestService.AssignEquipmentAsync(id, equipmentId, notes);
        if (request == null)
            return NotFound();

        return Ok(request);
    }

    /// <summary>
    /// Отменяет назначение оборудования на заявку
    /// </summary>
    /// <param name="id">Идентификатор заявки</param>
    /// <param name="equipmentId">Идентификатор оборудования</param>
    /// <returns>Обновленная заявка</returns>
    /// <response code="200">Назначение успешно отменено</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="404">Заявка не найдена</response>
    [HttpDelete("{id}/equipment/{equipmentId}/unassign")]
    public async Task<ActionResult<ServiceRequestDto>> UnassignEquipment(int id, Guid equipmentId)
    {
        var request = await _serviceRequestService.UnassignEquipmentAsync(id, equipmentId);
        if (request == null)
            return NotFound();

        return Ok(request);
    }
} 