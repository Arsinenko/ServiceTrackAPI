namespace AuthApp.application.DTOs;

public class UpdateUserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsAlive { get; set; }
    public Guid RoleId { get; set; }
}

public class UpdateUserBulkItemDto : UpdateUserDto
{
    public Guid UserId { get; set; }
}

public class UpdateBulkUserDto
{
    public required List<UpdateUserBulkItemDto> Users { get; set; }
}