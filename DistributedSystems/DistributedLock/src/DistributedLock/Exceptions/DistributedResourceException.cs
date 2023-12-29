namespace DistributedLock.Exceptions;

public class DistributedResourceException : Exception
{
    public DistributedResourceException(string? message) : base(message)
    {
    }

    public DistributedResourceException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}