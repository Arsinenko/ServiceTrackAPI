using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Api.Controllers;

/// <summary>
/// Контроллер для управления методами инспекции
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InspectionMethodController : ControllerBase
{
    private readonly IInspectionMethodService _inspectionMethodService;

    public InspectionMethodController(IInspectionMethodService inspectionMethodService)
    {
        _inspectionMethodService = inspectionMethodService;
    }

    /// <summary>
    /// Получает метод инспекции по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор метода инспекции</param>
    /// <returns>Метод инспекции</returns>
    /// <response code="200">Возвращает метод инспекции</response>
    /// <response code="404">Метод инспекции не найден</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<InspectionMethodDto>> Get(int id)
    {
        var result = await _inspectionMethodService.GetByIdAsync(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Получает список всех методов инспекции
    /// </summary>
    /// <returns>Список методов инспекции</returns>
    /// <response code="200">Возвращает список методов инспекции</response>
    [HttpGet]
    public async Task<ActionResult<List<InspectionMethodDto>>> GetAll()
    {
        var result = await _inspectionMethodService.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Получает метод инспекции по коду
    /// </summary>
    /// <param name="name">Код метода инспекции</param>
    /// <returns>Метод инспекции</returns>
    /// <response code="200">Возвращает метод инспекции</response>
    /// <response code="404">Метод инспекции не найден</response>
    [HttpGet("name/{name}")]
    public async Task<ActionResult<InspectionMethodDto>> GetByName(string name)
    {
        var result =  await _inspectionMethodService.GetByNameAsync(name);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Создает новый метод инспекции
    /// </summary>
    /// <param name="inspectionMethod">Данные для создания метода инспекции</param>
    /// <returns>Созданный метод инспекции</returns>
    /// <response code="201">Метод инспекции успешно создан</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="409">Метод инспекции с таким кодом уже существует</response>
    [HttpPost]
    public async Task<ActionResult<InspectionMethodDto>> Create(CreateInspectionMethodItemDto inspectionMethod)
    {
        var result = await _inspectionMethodService.CreateAsync(inspectionMethod);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    /// <summary>
    /// Создает несколько методов инспекции
    /// </summary>
    /// <param name="inspectionMethods">Данные для создания методов инспекции</param>
    /// <returns>Список созданных методов инспекции</returns>
    /// <response code="200">Методы инспекции успешно созданы</response>
    /// <response code="400">Некорректные данные</response>
    [HttpPost("bulk")]
    public async Task<ActionResult<List<InspectionMethodDto>>> BulkCreate(CreateInspectionMethodsDto inspectionMethods)
    {
        var result = await _inspectionMethodService.CreateBulkAsync(inspectionMethods);
        return Ok(result);
    }

    /// <summary>
    /// Обновляет несколько методов инспекции
    /// </summary>
    /// <param name="inspectionMethods">Данные для обновления методов инспекции</param>
    /// <returns>Список обновленных методов инспекции</returns>
    /// <response code="200">Методы инспекции успешно обновлены</response>
    /// <response code="400">Некорректные данные</response>
    [HttpPut("bulk")]
    public async Task<ActionResult<List<InspectionMethodDto>>> BulkUpdate(UpdateInspectionMethodsDto inspectionMethods)
    {
        var result = await _inspectionMethodService.UpdateBulkAsync(inspectionMethods);
        return Ok(result);
    }
}