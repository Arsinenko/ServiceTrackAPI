using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthApp.Api.Controllers;

/// <summary>
/// Контроллер для управления пользователями
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Получает информацию о текущем пользователе
    /// </summary>
    /// <returns>Информация о текущем пользователе</returns>
    /// <response code="200">Возвращает информацию о пользователе</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="404">Пользователь не найден</response>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Получает список всех пользователей
    /// </summary>
    /// <returns>Список пользователей</returns>
    /// <response code="200">Возвращает список пользователей</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа (требуется роль Admin)</response>
    [HttpGet]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    /// <summary>
    /// Получает пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Информация о пользователе</returns>
    /// <response code="200">Возвращает информацию о пользователе</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Получает пользователя по email
    /// </summary>
    /// <param name="email">Email пользователя</param>
    /// <returns>Информация о пользователе</returns>
    /// <response code="200">Возвращает информацию о пользователе</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpGet("email/{email}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetByEmail(string email)
    {
        var user = await _userService.GetByEmailAsync(email);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Обновляет информацию о пользователе
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <param name="updateUserDto">Данные для обновления</param>
    /// <returns>Обновленная информация о пользователе</returns>
    /// <response code="200">Информация успешно обновлена</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Update(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _userService.UpdateAsync(id, updateUserDto);
        if (user == null)
            return NotFound();

        return Ok(user);
    }
    /// <summary>
    /// Обновляет информацию о текущем пользователе
    /// </summary>
    /// <param name="updateUserDto">Данные для обновления</param>
    /// <returns>Обновленная информация о пользователе</returns>
    /// <response code="200">Информация успешно обновлена</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="404">Пользователь не найден</response>
    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateCurrentUser(UpdateUserDto updateUserDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }
        var user = await _userService.UpdateAsync(userId, updateUserDto);
        return Ok(user);
    }

    /// <summary>
    /// Удаляет пользователя (мягкое удаление)
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Нет содержимого</returns>
    /// <response code="204">Пользователь успешно удален</response>
    /// <response code="401">Требуется авторизация</response>
    /// <response code="403">Нет прав доступа (требуется роль Admin)</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
} 