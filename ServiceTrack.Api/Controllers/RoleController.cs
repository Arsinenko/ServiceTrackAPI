using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Api.Controllers;

/// <summary>
/// Контроллер для управления ролями пользователей
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Получает список всех ролей
    /// </summary>
    /// <returns>Список ролей</returns>
    /// <response code="200">Возвращает список ролей</response>
    /// <response code="401">Требуется авторизация</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
    {
        var roles = await _roleService.GetAllAsync();
        return Ok(roles);
    }

    /// <summary>
    /// Получает роль по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор роли</param>
    /// <returns>Информация о роли</returns>
    /// <response code="200">Возвращает информацию о роли</response>
    /// <response code="401">Требуется авторизация</response>
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
    /// <response code="400">Некорректные данные (пустое имя/описание, превышена максимальная длина)</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа (требуется роль Admin)</response>
    /// <response code="409">Роль с таким именем уже существует</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create(CreateRoleDto createRoleDto)
    {
        var role = await _roleService.CreateAsync(createRoleDto);
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    /// <summary>
    /// Создает несколько ролей
    /// </summary>
    /// <param name="createRoleBulkDto">Данные для создания ролей</param>
    /// <returns>Результат создания ролей (успешные и неуспешные)</returns>
    /// <response code="201">Роли успешно созданы (частично или полностью)</response>
    /// <response code="400">Некорректные данные (пустые имена/описания, превышена максимальная длина)</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа (требуется роль Admin)</response>
    /// <response code="409">Некоторые роли не созданы из-за конфликта имен</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("bulk")]
    public async Task<ActionResult<CreateRoleBulkResultDto>> CreateBulkAsync(CreateRoleBulkDto createRoleBulkDto)
    {
        var roles = await _roleService.CreateBulkAsync(createRoleBulkDto);
        return CreatedAtAction(nameof(GetAll), roles);
    }
    
    /// <summary>
    /// Обновляет существующую роль (можно передавать только изменяемые поля)
    /// </summary>
    /// <param name="id">Идентификатор роли</param>
    /// <param name="updateRoleDto">Данные для обновления роли (только изменяемые поля)</param>
    /// <returns>Обновленная роль</returns>
    /// <response code="200">Роль успешно обновлена</response>
    /// <response code="400">Некорректные данные (пустое имя/описание, превышена максимальная длина)</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа (требуется роль Admin)</response>
    /// <response code="404">Роль не найдена</response>
    /// <response code="409">Роль с новым именем уже существует</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<RoleDto>> Update(Guid id, UpdateRoleDto updateRoleDto)
    {
        var role = await _roleService.UpdateAsync(id, updateRoleDto);
        return Ok(role);
    }

    /// <summary>
    /// Обновляет несколько ролей
    /// </summary>
    /// <param name="updateRoleBulkDto">Данные для обновления ролей</param>
    /// <returns>Обновленные роли</returns>
    /// <response code="200">Роли успешно обновлены</response>
    /// <response code="400">Некорректные данные (пустые имена/описания, превышена максимальная длина)</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа (требуется роль Admin)</response>
    /// <response code="404">Некоторые роли не найдены</response>
    /// <response code="409">Некоторые роли не обновлены из-за конфликта имен</response>
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
    /// <response code="403">Нет прав доступа (требуется роль Admin)</response>
    /// <response code="404">Роль не найдена</response>
    /// <response code="409">Невозможно удалить роль, так как она используется пользователями</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _roleService.DeleteAsync(id);
        return NoContent();
    }
} 