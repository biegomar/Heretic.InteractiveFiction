namespace Heretic.InteractiveFiction.Exceptions;

public class PutOnException: Exception
{
    public PutOnException()
    {
    }

    public PutOnException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public PutOnException(string message) : base(message)
    {
    }

    public PutOnException(string message, Exception innerException) : base(message, innerException)
    {
    }
}