using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }
    /// <summary>
    /// Получает все роли. 
    /// </summary>
    /// <returns>Список ролей</returns>
    /// <response code="200">Возвращает список ролей</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
    {
        var roles = await _roleService.GetAllAsync();
        return Ok(roles);
    }
    /// <summary>
    /// Получает роль по id
    /// </summary>
    /// <param name="id">Идентификатор роли</param>
    /// <returns>Роль</returns>
    /// <response code="200">Возвращает роль</response>
    /// <response code="404">Роль не найдена</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetById(Guid id)
    {
        var role = await _roleService.GetByIdAsync(id);
        if (role == null)
            return NotFound();

        return Ok(role);
    }
    /// <summary>
    /// Создает новую роль
    /// </summary>
    /// <param name="createRoleDto">Данные для создания роли</param>
    /// <returns>Созданная роль</returns>
    /// <response code="201">Роль успешно создана</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create(CreateRoleDto createRoleDto)
    {
        var role = await _roleService.CreateAsync(createRoleDto);
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    /// <summary>
    /// Создает роли
    /// </summary>
    /// <param name="createRoleBulkDto">Данные для создания ролей</param>
    /// <returns>Созданные роли</returns>
    /// <response code="201">Роли успешно созданы</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("bulk")]
    public async Task<ActionResult<CreateRoleBulkResultDto>> CreateBulkAsync(CreateRoleBulkDto createRoleBulkDto)
    {
        var roles = await _roleService.CreateBulkAsync(createRoleBulkDto);
        return CreatedAtAction(nameof(GetAll), roles);
    }
    
    /// <summary>
    /// Обновляет существующую роль
    /// </summary>
    /// <param name="id">Идентификатор роли</param>
    /// <param name="updateRoleDto">Данные для обновления роли</param>
    /// <returns>Обновленная роль</returns>
    /// <response code="200">Роль успешно обновлена</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа</response>
    /// <response code="404">Роль не найдена</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<RoleDto>> Update(Guid id, UpdateRoleDto updateRoleDto)
    {
        var role = await _roleService.UpdateAsync(id, updateRoleDto);
        if (role == null)
            return NotFound();

        return Ok(role);
    }

    /// <summary>
    /// Обновляет существующие роли
    /// </summary>
    /// <param name="updateRoleBulkDto">Данные для обновления</param>
    /// <returns>Обновленные роли</returns>
    /// <response code="200">Роль успешно обновлена</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа</response>
    /// <response code="404">Роль не найдена</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("bulk")]
    public async Task<ActionResult<List<RoleDto>>> UpdateBulkAsync(UpdateRoleBulkDto updateRoleBulkDto)
    {
        var roles = await _roleService.UpdateBulkAsync(updateRoleBulkDto);
        return Ok(roles);
    }
    
    /// <summary>
    /// Удаляет роль
    /// </summary>
    /// <param name="id">Идентификатор роли</param>
    /// <returns>Нет содержимого</returns>
    /// <response code="204">Роль успешно удалена</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _roleService.DeleteAsync(id);
        return NoContent();
    }
} 