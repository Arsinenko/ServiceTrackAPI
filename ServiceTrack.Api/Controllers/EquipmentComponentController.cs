using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Api.Controllers;

/// <summary>
/// Контроллер для управления компонентами оборудования
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EquipmentComponentController : ControllerBase
{
    private readonly IEquipmentComponentService _componentService;

    public EquipmentComponentController(IEquipmentComponentService componentService)
    {
        _componentService = componentService;
    }

    /// <summary>
    /// Получает список всех компонентов оборудования
    /// </summary>
    /// <returns>Список компонентов оборудования</returns>
    /// <response code="200">Возвращает список компонентов</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EquipmentComponentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EquipmentComponentDto>>> GetAllComponents()
    {
        var components = await _componentService.GetAllAsync();
        return Ok(components);
    }

    /// <summary>
    /// Получает компонент оборудования по его идентификатору
    /// </summary>
    /// <param name="id">Идентификатор компонента</param>
    /// <returns>Компонент оборудования</returns>
    /// <response code="200">Возвращает компонент</response>
    /// <response code="404">Если компонент не найден</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EquipmentComponentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipmentComponentDto>> GetComponentById(Guid id)
    {
        var component = await _componentService.GetByIdAsync(id);
        if (component == null)
        {
            return NotFound();
        }
        return Ok(component);
    }

    /// <summary>
    /// Получает список компонентов для конкретного оборудования
    /// </summary>
    /// <param name="equipmentId">Идентификатор оборудования</param>
    /// <returns>Список компонентов оборудования</returns>
    /// <response code="200">Возвращает список компонентов</response>
    [HttpGet("equipment/{equipmentId}")]
    [ProducesResponseType(typeof(IEnumerable<EquipmentComponentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EquipmentComponentDto>>> GetComponentsByEquipmentId(Guid equipmentId)
    {
        var components = await _componentService.GetByEquipmentIdAsync(equipmentId);
        return Ok(components);
    }

    /// <summary>
    /// Создает новый компонент оборудования
    /// </summary>
    /// <param name="createComponentDto">Данные для создания компонента</param>
    /// <returns>Созданный компонент</returns>
    /// <response code="201">Возвращает созданный компонент</response>
    /// <response code="400">Если данные некорректны</response>
    [HttpPost]
    [ProducesResponseType(typeof(EquipmentComponentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EquipmentComponentDto>> CreateComponent(CreateEquipmentComponentDto createComponentDto)
    {
        var component = await _componentService.CreateAsync(createComponentDto);
        return CreatedAtAction(nameof(GetComponentById), new { id = component.Id }, component);
    }

    /// <summary>
    /// Обновляет существующий компонент оборудования
    /// </summary>
    /// <param name="id">Идентификатор компонента</param>
    /// <param name="updateComponentDto">Данные для обновления компонента</param>
    /// <returns>Обновленный компонент</returns>
    /// <response code="200">Возвращает обновленный компонент</response>
    /// <response code="404">Если компонент не найден</response>
    /// <response code="400">Если данные некорректны</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(EquipmentComponentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EquipmentComponentDto>> UpdateComponent(Guid id, UpdateEquipmentComponentDto updateComponentDto)
    {
        var component = await _componentService.UpdateAsync(id, updateComponentDto);
        if (component == null)
        {
            return NotFound();
        }
        return Ok(component);
    }

    /// <summary>
    /// Удаляет компонент оборудования
    /// </summary>
    /// <param name="id">Идентификатор компонента</param>
    /// <returns>Нет содержимого</returns>
    /// <response code="204">Если компонент успешно удален</response>
    /// <response code="404">Если компонент не найден</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComponent(Guid id)
    {
        await _componentService.DeleteAsync(id);
        return NoContent();
    }
} 