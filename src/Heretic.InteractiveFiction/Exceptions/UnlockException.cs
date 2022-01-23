namespace Heretic.InteractiveFiction.Exceptions;

public class UnlockException: Exception
{
    public UnlockException()
    {
    }

    public UnlockException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public UnlockException(string message) : base(message)
    {
    }

    public UnlockException(string message, Exception innerException) : base(message, innerException)
    {
    }
}