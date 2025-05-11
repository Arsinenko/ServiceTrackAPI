namespace AuthApp.application.Exceptions;

public class RoleValidationException : Exception
{
    public RoleValidationException(string message) : base(message)
    {
    }
}

public class RoleNotFoundException : Exception
{
    public RoleNotFoundException(string message) : base(message)
    {
    }
}

public class RoleNameAlreadyExistsException : Exception
{
    public RoleNameAlreadyExistsException(string message) : base(message)
    {
    }
}

public class RoleInUseException : Exception
{
    public RoleInUseException(string message) : base(message)
    {
    }
} 