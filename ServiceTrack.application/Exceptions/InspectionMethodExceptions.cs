namespace AuthApp.application.Exceptions;public abstract class CreateInspectionMethodItemDto
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class InspectionMethodValidationException : Exception
{
    public InspectionMethodValidationException(string message) : base(message)
    {
    }
}

public class InspectionMethodNotFoundException : Exception
{
    public InspectionMethodNotFoundException(string message) : base(message)
    {
    }
}

public class InspectionMethodNameAlreadyExistsException : Exception
{
    public InspectionMethodNameAlreadyExistsException(string message) : base(message)
    {
    }
}

public class InspectionMethodInUseException : Exception
{
    public InspectionMethodInUseException(string message) : base(message)
    {
    }
} 