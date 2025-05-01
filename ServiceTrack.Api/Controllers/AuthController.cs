using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService )
    {
        _authService = authService;
    }

    /// <summary>
    /// Регистрирует нового пользователя
    /// </summary>
    /// <param name="registerUserDto">Данные для регистрации пользователя</param>
    /// <returns>Результат регистрации</returns>
    /// <response code="200">Пользователь успешно зарегистрирован</response>
    /// <response code="400">Некорректные данные или ошибка регистрации</response>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResult>> Register(RegisterUserDto registerUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _authService.RegisterAsync(registerUserDto);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Выполняет вход пользователя в систему
    /// </summary>
    /// <param name="loginUserDto">Данные для входа</param>
    /// <returns>Результат аутентификации</returns>
    /// <response code="200">Успешный вход в систему</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="401">Неверные учетные данные</response>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResult>> Login(LoginUserDto loginUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _authService.LoginAsync(loginUserDto);
        if (!result.Success)
        {
            return Unauthorized(result);
        }
        return Ok(result);
    }
}