using System.ComponentModel.DataAnnotations;
using AuthApp.domain.Entities;

namespace AuthApp.application.DTOs;

public class SecurityLevelDto
{
    public int Id { get; set; }
    public required string Code { get; set; } = null!;
    public required string Name { get; set; } = null!;
    public string? Description { get; set; }
    public required bool IsAlive { get; set; }

    public static SecurityLevelDto FromSecurityLevel(SecurityLevel securityLevel)
    {
        return new SecurityLevelDto
        {
            Id = securityLevel.Id,
            Code = securityLevel.Code,
            Name = securityLevel.Name,
            Description = securityLevel.Description,
            IsAlive = securityLevel.IsAlive
        };
    }
}

public class CreateSecurityLevelDto
{
    [Required(ErrorMessage = "Код обязателен")]
    [StringLength(50, ErrorMessage = "Код не может быть длиннее 50 символов")]
    public required string Code { get; set; } = null!;

    [Required(ErrorMessage = "Наименование обязательно")]
    [StringLength(100, ErrorMessage = "Наименование не может быть длиннее 100 символов")]
    public required string Name { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Описание не может быть длиннее 500 символов")]
    public string? Description { get; set; }

    public required bool IsAlive { get; set; }
}

public class UpdateSecurityLevelDto : CreateSecurityLevelDto;

public class BulkCreateSecurityLevelDto
{
    [Required(ErrorMessage = "Список уровней безопасности обязателен")]
    [MinLength(1, ErrorMessage = "Список не может быть пустым")]
    public List<CreateSecurityLevelDto> Items { get; set; }
}

public class BulkUpdateSecurityLevelDto
{
    [Required(ErrorMessage = "Список уровней безопасности обязателен")]
    [MinLength(1, ErrorMessage = "Список не может быть пустым")]
    public List<BulkUpdateSecurityLevelItemDto> Items { get; set; }
}

public class BulkUpdateSecurityLevelItemDto
{
    [Required(ErrorMessage = "Идентификатор обязателен")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Код обязателен")]
    [StringLength(50, ErrorMessage = "Код не может быть длиннее 50 символов")]
    public string Code { get; set; }

    [Required(ErrorMessage = "Наименование обязательно")]
    [StringLength(100, ErrorMessage = "Наименование не может быть длиннее 100 символов")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Описание не может быть длиннее 500 символов")]
    public string Description { get; set; }
    public required bool IsAlive { get; set; }
}