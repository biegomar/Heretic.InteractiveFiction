namespace Heretic.InteractiveFiction.Exceptions;

public class WaitException: Exception
{
    public WaitException()
    {
    }

    public WaitException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public WaitException(string message) : base(message)
    {
    }

    public WaitException(string message, Exception innerException) : base(message, innerException)
    {
    }
}