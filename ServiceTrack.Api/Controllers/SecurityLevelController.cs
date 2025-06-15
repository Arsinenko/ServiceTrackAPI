using AuthApp.application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceTrack.application.Interfaces;

namespace AuthApp.Api.Controllers;

/// <summary>
/// Контроллер для управления уровнями безопасности
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SecurityLevelController : ControllerBase
{
    private readonly ISecurityLevelService _securityLevelService;

    public SecurityLevelController(ISecurityLevelService securityLevelService)
    {
        _securityLevelService = securityLevelService;
    }

    /// <summary>
    /// Получает список всех уровней безопасности
    /// </summary>
    /// <returns>Список уровней безопасности</returns>
    /// <response code="200">Возвращает список уровней безопасности</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SecurityLevelDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SecurityLevelDto>>> GetAll()
    {
        var securityLevels = await _securityLevelService.GetAllAsync();
        return Ok(securityLevels);
    }

    /// <summary>
    /// Получает уровень безопасности по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор уровня безопасности</param>
    /// <returns>Уровень безопасности</returns>
    /// <response code="200">Возвращает уровень безопасности</response>
    /// <response code="404">Если уровень безопасности не найден</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SecurityLevelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SecurityLevelDto>> GetById(int id)
    {
        var securityLevel = await _securityLevelService.GetByIdAsync(id);
        return Ok(securityLevel);
    }

    /// <summary>
    /// Создает новый уровень безопасности
    /// </summary>
    /// <param name="createDto">Данные для создания уровня безопасности</param>
    /// <returns>Созданный уровень безопасности</returns>
    /// <response code="201">Возвращает созданный уровень безопасности</response>
    /// <response code="400">Если данные некорректны</response>
    /// <response code="409">Если уровень безопасности с таким кодом или именем уже существует</response>
    [HttpPost]
    [ProducesResponseType(typeof(SecurityLevelDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SecurityLevelDto>> Create(CreateSecurityLevelDto createDto)
    {
        var createdSecurityLevel = await _securityLevelService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = createdSecurityLevel.Id }, createdSecurityLevel);
    }

    /// <summary>
    /// Массовое создание уровней безопасности
    /// </summary>
    /// <param name="items">Данные для массового создания уровней безопасности</param>
    /// <returns>Список созданных уровней безопасности</returns>
    /// <response code="201">Возвращает список созданных уровней безопасности</response>
    /// <response code="400">Если данные некорректны</response>
    /// <response code="409">Если один или несколько уровней безопасности с таким кодом или именем уже существуют</response>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(IEnumerable<SecurityLevelDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<IEnumerable<SecurityLevelDto>>> BulkCreate(BulkCreateSecurityLevelDto items)
    {
        var createdSecurityLevels = await _securityLevelService.BulkCreateAsync(items);
        return CreatedAtAction(nameof(GetAll), createdSecurityLevels);
    }

    /// <summary>
    /// Обновляет существующий уровень безопасности
    /// </summary>
    /// <param name="id">Идентификатор уровня безопасности</param>
    /// <param name="updateDto">Данные для обновления уровня безопасности</param>
    /// <returns>Обновленный уровень безопасности</returns>
    /// <response code="200">Возвращает обновленный уровень безопасности</response>
    /// <response code="400">Если данные некорректны</response>
    /// <response code="404">Если уровень безопасности не найден</response>
    /// <response code="409">Если уровень безопасности с таким кодом или именем уже существует</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SecurityLevelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SecurityLevelDto>> Update(int id, UpdateSecurityLevelDto updateDto)
    {
        var updatedSecurityLevel = await _securityLevelService.UpdateAsync(id, updateDto);
        return Ok(updatedSecurityLevel);
    }

    /// <summary>
    /// Массовое обновление уровней безопасности
    /// </summary>
    /// <param name="bulkUpdateDto">Данные для массового обновления уровней безопасности</param>
    /// <returns>Список обновленных уровней безопасности</returns>
    /// <response code="200">Возвращает список обновленных уровней безопасности</response>
    /// <response code="400">Если данные некорректны</response>
    /// <response code="404">Если один или несколько уровней безопасности не найдены</response>
    /// <response code="409">Если один или несколько уровней безопасности с таким кодом или именем уже существуют</response>
    [HttpPut("bulk")]
    [ProducesResponseType(typeof(IEnumerable<SecurityLevelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<IEnumerable<SecurityLevelDto>>> BulkUpdate(BulkUpdateSecurityLevelDto bulkUpdateDto)
    {
        var updatedSecurityLevels = await _securityLevelService.BulkUpdateAsync(bulkUpdateDto);
        return Ok(updatedSecurityLevels);
    }

    /// <summary>
    /// Удаляет уровень безопасности
    /// </summary>
    /// <param name="id">Идентификатор уровня безопасности</param>
    /// <returns>Результат операции</returns>
    /// <response code="204">Если уровень безопасности успешно удален</response>
    /// <response code="404">Если уровень безопасности не найден</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _securityLevelService.DeleteAsync(id);
        return NoContent();
    }
} 