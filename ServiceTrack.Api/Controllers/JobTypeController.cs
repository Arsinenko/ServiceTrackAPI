using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Api.Controllers;

/// <summary>
/// Контроллер для управления типами работ
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class JobTypeController : ControllerBase
{
    private readonly IJobTypeService _jobTypeService;

    public JobTypeController(IJobTypeService jobTypeService)
    {
        _jobTypeService = jobTypeService;
    }

    /// <summary>
    /// Получает тип работы по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор типа работы</param>
    /// <returns>Тип работы</returns>
    /// <response code="200">Возвращает тип работы</response>
    /// <response code="404">Тип работы не найден</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<JobTypeDto>> GetById(Guid id)
    {
        var jobType = await _jobTypeService.GetByIdAsync(id);
        if (jobType == null)
            return NotFound();
        return Ok(jobType);
    }

    /// <summary>
    /// Получает тип работы по названию
    /// </summary>
    /// <param name="name">Название типа работы</param>
    /// <returns>Тип работы</returns>
    /// <response code="200">Возвращает тип работы</response>
    /// <response code="404">Тип работы не найден</response>
    [HttpGet("name/{name}")]
    public async Task<ActionResult<JobTypeDto>> GetByName(string name)
    {
        var jobType = await _jobTypeService.GetBeyNameAsync(name);
        if (jobType == null)
            return NotFound();
        return Ok(jobType);
    }

    /// <summary>
    /// Получает список всех типов работ
    /// </summary>
    /// <returns>Список типов работ</returns>
    /// <response code="200">Возвращает список типов работ</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobTypeDto>>> GetAll()
    {
        var jobTypes = await _jobTypeService.GetAllAsync();
        return Ok(jobTypes);
    }

    /// <summary>
    /// Создает новый тип работы
    /// </summary>
    /// <param name="jobTypeDto">Данные для создания типа работы</param>
    /// <returns>Созданный тип работы</returns>
    /// <response code="201">Тип работы успешно создан</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа (требуется роль Admin)</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<JobTypeDto>> Create(CreateJobTypeDto jobTypeDto)
    {
        var jobType = await _jobTypeService.CreateAsync(jobTypeDto);
        return CreatedAtAction(nameof(GetById), new { id = jobType.Id }, jobType);
    }
    
    /// <summary>
    /// Создает несколько типов работы
    /// </summary>
    /// <param name="jobTypeDto">Данные для создания типов работы</param>
    /// <returns>Созданный тип работы</returns>
    /// <response code="201">Типы работы успешно созданы</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа (требуется роль Admin)</response>
    /*[Authorize(Roles = "Admin")]*/ //TODO
    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<JobTypeDto>>> CreateBulkAsync(CreateJobTypeBulkDto jobTypeBulkDto)
    {
        var jobTypes = await _jobTypeService.CreateBulkAsync(jobTypeBulkDto);
        return CreatedAtAction(nameof(GetAll), jobTypes);
    }

    /// <summary>
    /// Обновляет существующий тип работы
    /// </summary>
    /// <param name="id">Идентификатор типа работы</param>
    /// <param name="jobTypeDto">Данные для обновления типа работы</param>
    /// <returns>Обновленный тип работы</returns>
    /// <response code="200">Тип работы успешно обновлен</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа (требуется роль Admin)</response>
    /// <response code="404">Тип работы не найден</response>
    // [Authorize(Roles = "Admin")] TODO
    [HttpPut("{id}")]
    public async Task<ActionResult<JobTypeDto>> Update(Guid id, UpdateJobTypeDto jobTypeDto)
    {
        var jobType = await _jobTypeService.UpdateAsync(id, jobTypeDto);
        if (jobType == null)
        {
            return NotFound();
        }
        return Ok(jobType);
    }

    /// <summary>
    /// Обновляет существующие типы работы
    /// </summary>
    /// <param name="jobTypeBulkDto">Данные для обновления типов работы</param>
    /// <returns>Обновленный тип работы</returns>
    /// <response code="200">Типы работы успешно обновлены</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа (требуется роль Admin)</response>
    /// <response code="404">Тип работы не найден</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("bulk")]
    public async Task<ActionResult<List<JobTypeDto>>> UpdateBulkAsync(UpdateJobTypeBulkDto jobTypeBulkDto)
    {
        var jobTypes = await _jobTypeService.UpdateBulkAsync(jobTypeBulkDto);
        if (jobTypes == null)
        {
            return NotFound();
        }
        return Ok(jobTypes);
    }
}