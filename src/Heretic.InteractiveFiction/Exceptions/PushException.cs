namespace Heretic.InteractiveFiction.Exceptions;

public class PushException: Exception
{
    public PushException()
    {
    }

    public PushException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public PushException(string message) : base(message)
    {
    }

    public PushException(string message, Exception innerException) : base(message, innerException)
    {
    }
}