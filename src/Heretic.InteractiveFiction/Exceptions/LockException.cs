namespace Heretic.InteractiveFiction.Exceptions;

public class LockException: Exception
{
    public LockException()
    {
    }

    public LockException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public LockException(string message) : base(message)
    {
    }

    public LockException(string message, Exception innerException) : base(message, innerException)
    {
    } 
}