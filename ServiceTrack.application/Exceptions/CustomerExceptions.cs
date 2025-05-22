namespace AuthApp.application.Exceptions;

public class CustomerValidationException : Exception
{
    public CustomerValidationException(string message) : base(message)
    {
    }
}

public class CustomerNotFoundException : Exception
{
    public CustomerNotFoundException(string message) : base(message)
    {
    }
}

public class CustomerAlreadyExistsException : Exception
{
    public CustomerAlreadyExistsException(string message) : base(message)
    {
    }
}

public class CustomerInUseException : Exception
{
    public CustomerInUseException(string message) : base(message)
    {
    }
}