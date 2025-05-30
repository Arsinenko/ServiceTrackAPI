namespace ServiceTrack.application.Exceptions;

public class SecurityLevelAlreadyExistsException : Exception
{
    public SecurityLevelAlreadyExistsException(string message) : base(message)
    {
    }
} 