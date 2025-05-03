using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentServiceservice;

    public EquipmentController(IEquipmentService equipmentServiceservice)
    {
        _equipmentServiceservice = equipmentServiceservice;
    }

    /// <summary>
    /// Получает все оборудование
    /// </summary>
    /// <returns>Список оборудования</returns>
    /// <response code="200">Возвращает список оборудования</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EquipmentDto>>> GetAllEquipment()
    {
        var equipment = await _equipmentServiceservice.GetAllAsync();
        return Ok(equipment);
    }

    /// <summary>
    /// Получает оборудование по id
    /// </summary>
    /// <param name="id">Идентификатор оборудования</param>
    /// <returns>Оборудование</returns>
    /// <response code="200">Возвращает оборудование</response>
    /// <response code="404">Оборудование не найдено</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<EquipmentDto>> GetEquipmentById(Guid id)
    {
        var equipment = await _equipmentServiceservice.GetByIdAsync(id);
        if (equipment == null)
            return NotFound();
        return Ok(equipment);
    }
    
    /// <summary>
    /// Создает новое оборудование
    /// </summary>
    /// <param name="createEquipmentDto">Данные для создания оборудования</param>
    /// <returns>Созданное оборудование</returns>
    /// <response code="201">Оборудование успешно создано</response>
    /// <response code="400">Некорректные данные</response>
    [HttpPost]
    public async Task<ActionResult<EquipmentDto>> CreateEquipment(CreateEquipmentDto createEquipmentDto)
    {
        var equipment = await _equipmentServiceservice.CreateAsync(createEquipmentDto);
        return CreatedAtAction(nameof(GetEquipmentById), new { id = equipment.Id }, equipment);
    }

    /// <summary>
    /// Обновляет существующее оборудование
    /// </summary>
    /// <param name="id">Идентификатор оборудования</param>
    /// <param name="updateEquipmentDto">Данные для обновления оборудования</param>
    /// <returns>Обновленное оборудование</returns>
    /// <response code="200">Оборудование успешно обновлено</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="404">Оборудование не найдено</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<EquipmentDto>> UpdateEquipment(Guid id, UpdateEquipmentDto updateEquipmentDto)
    {
        var equipment = await _equipmentServiceservice.UpdateAsync(id, updateEquipmentDto);
        if (equipment == null)
            return NotFound();
        return Ok(equipment);
    }

    /// <summary>
    /// Удаляет оборудование
    /// </summary>
    /// <param name="id">Идентификатор оборудования</param>
    /// <returns>Нет содержимого</returns>
    /// <response code="204">Оборудование успешно удалено</response>
    /// <response code="404">Оборудование не найдено</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEquipment(Guid id)
    {
        await _equipmentServiceservice.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Получает компонент оборудования
    /// </summary>
    /// <param name="equipmentId">Идентификатор оборудования</param>
    /// <param name="componentId">Идентификатор компонента</param>
    /// <returns>Компонент оборудования</returns>
    /// <response code="200">Возвращает компонент</response>
    /// <response code="404">Компонент не найден</response>
    [HttpGet("{equipmentId}/components/{componentId}")]
    public async Task<ActionResult<EquipmentDto>> GetComponent(Guid equipmentId, Guid componentId)
    {
        var component = await _equipmentServiceservice.GetComponentAsync(equipmentId, componentId);
        if (component == null)
            return NotFound();
        return Ok(component);
    }

    /// <summary>
    /// Добавляет компонент к оборудованию
    /// </summary>
    /// <param name="equipmentId">Идентификатор оборудования</param>
    /// <param name="componentDto">Данные компонента</param>
    /// <returns>Обновленное оборудование</returns>
    /// <response code="200">Компонент успешно добавлен</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="404">Оборудование не найдено</response>
    [HttpPost("{equipmentId}/components")]
    public async Task<ActionResult<EquipmentDto>> AddComponent(Guid equipmentId, CreateEquipmentDto componentDto)
    {
        var equipment = await _equipmentServiceservice.AddComponentAsync(equipmentId, componentDto);
        if (equipment == null)
            return NotFound();
        return Ok(equipment);
    }

    /// <summary>
    /// Обновляет компонент оборудования
    /// </summary>
    /// <param name="equipmentId">Идентификатор оборудования</param>
    /// <param name="componentId">Идентификатор компонента</param>
    /// <param name="componentDto">Данные для обновления компонента</param>
    /// <returns>Обновленное оборудование</returns>
    /// <response code="200">Компонент успешно обновлен</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="404">Компонент не найден</response>
    [HttpPut("{equipmentId}/components/{componentId}")]
    public async Task<ActionResult<EquipmentDto>> UpdateComponent(Guid equipmentId, Guid componentId, UpdateEquipmentDto componentDto)
    {
        var equipment = await _equipmentServiceservice.UpdateComponentAsync(equipmentId, componentId, componentDto);
        if (equipment == null)
            return NotFound();
        return Ok(equipment);
    }

    /// <summary>
    /// Удаляет компонент из оборудования
    /// </summary>
    /// <param name="equipmentId">Идентификатор оборудования</param>
    /// <param name="componentId">Идентификатор компонента</param>
    /// <returns>Нет содержимого</returns>
    /// <response code="204">Компонент успешно удален</response>
    /// <response code="404">Компонент не найден</response>
    [HttpDelete("{equipmentId}/components/{componentId}")]
    public async Task<IActionResult> RemoveComponent(Guid equipmentId, Guid componentId)
    {
        var result = await _equipmentServiceservice.RemoveComponentAsync(equipmentId, componentId);
        if (!result)
            return NotFound();
        return NoContent();
    }
}