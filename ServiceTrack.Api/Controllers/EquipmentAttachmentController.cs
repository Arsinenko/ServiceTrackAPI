using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Api.Controllers;

/// <summary>
/// Контроллер для управления вложениями оборудования
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class EquipmentAttachmentController : ControllerBase
{
    private readonly IEquipmentAttachmentService _attachmentService;

    public EquipmentAttachmentController(IEquipmentAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    /// <summary>
    /// Загружает файл для оборудования
    /// </summary>
    /// <param name="equipmentId">Идентификатор оборудования</param>
    /// <param name="file">Файл для загрузки</param>
    /// <param name="description">Описание вложения (опционально)</param>
    /// <returns>Информация о загруженном вложении</returns>
    /// <response code="201">Файл успешно загружен</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="404">Оборудование не найдено</response>
    [HttpPost("{equipmentId}")]
    public async Task<ActionResult<EquipmentAttachmentDto>> UploadAttachment(
        Guid equipmentId,
        IFormFile file,
        [FromQuery] string? description = null)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не был предоставлен");

        var attachment = await _attachmentService.SaveAttachmentAsync(file, equipmentId, description);
        return CreatedAtAction(nameof(GetAttachment), new { id = attachment.Id }, EquipmentAttachmentDto.FromEquipmentAttachment(attachment));
    }

    /// <summary>
    /// Загружает несколько файлов для оборудования
    /// </summary>
    /// <param name="equipmentId">Идентификатор оборудования</param>
    /// <param name="files">Файлы для загрузки</param>
    /// <param name="description">Описание вложений (опционально)</param>
    /// <returns>Список загруженных вложений</returns>
    /// <response code="201">Файлы успешно загружены</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="404">Оборудование не найдено</response>
    [HttpPost("{equipmentId}/bulk")]
    public async Task<ActionResult<List<EquipmentAttachmentDto>>> UploadAttachments(
        Guid equipmentId,
        List<IFormFile> files,
        [FromQuery] string? description = null)
    {
        if (files == null || !files.Any())
            return BadRequest("Файлы не были предоставлены");

        var attachments = await _attachmentService.SaveAttachmentsAsync(files, equipmentId, description);
        return CreatedAtAction(nameof(GetAttachments), new { equipmentId }, 
            attachments.Select(a => EquipmentAttachmentDto.FromEquipmentAttachment(a)).ToList());
    }

    /// <summary>
    /// Получает информацию о вложении
    /// </summary>
    /// <param name="id">Идентификатор вложения</param>
    /// <returns>Информация о вложении</returns>
    /// <response code="200">Возвращает информацию о вложении</response>
    /// <response code="404">Вложение не найдено</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<EquipmentAttachmentDto>> GetAttachment(int id)
    {
        var attachment = await _attachmentService.GetByIdAsync(id);
        if (attachment == null)
            return NotFound();

        return Ok(EquipmentAttachmentDto.FromEquipmentAttachment(attachment));
    }

    /// <summary>
    /// Получает список вложений для оборудования
    /// </summary>
    /// <param name="equipmentId">Идентификатор оборудования</param>
    /// <returns>Список вложений</returns>
    /// <response code="200">Возвращает список вложений</response>
    /// <response code="404">Оборудование не найдено</response>
    [HttpGet("equipment/{equipmentId}")]
    public async Task<ActionResult<List<EquipmentAttachmentDto>>> GetAttachments(Guid equipmentId)
    {
        var attachments = await _attachmentService.GetAttachmentsByEquipmentIdAsync(equipmentId);
        return Ok(attachments.Select(a => EquipmentAttachmentDto.FromEquipmentAttachment(a)).ToList());
    }

    /// <summary>
    /// Удаляет вложение
    /// </summary>
    /// <param name="id">Идентификатор вложения</param>
    /// <returns>Результат операции</returns>
    /// <response code="204">Вложение успешно удалено</response>
    /// <response code="404">Вложение не найдено</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAttachment(int id)
    {
        await _attachmentService.DeleteAttachmentAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Удаляет несколько вложений
    /// </summary>
    /// <param name="ids">Список идентификаторов вложений</param>
    /// <returns>Результат операции</returns>
    /// <response code="204">Вложения успешно удалены</response>
    /// <response code="400">Некорректные данные</response>
    [HttpDelete("bulk")]
    public async Task<IActionResult> DeleteAttachments([FromBody] List<int> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest("Список идентификаторов не был предоставлен");

        await _attachmentService.DeleteAttachmentsAsync(ids);
        return NoContent();
    }

    /// <summary>
    /// Скачивает файл вложения
    /// </summary>
    /// <param name="id">Идентификатор вложения</param>
    /// <returns>Файл для скачивания</returns>
    /// <response code="200">Возвращает файл</response>
    /// <response code="404">Вложение не найдено</response>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadAttachment(int id)
    {
        try
        {
            var (fileContent, fileName, contentType) = await _attachmentService.GetAttachmentFileAsync(id);
            return File(fileContent, contentType, fileName);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
} 