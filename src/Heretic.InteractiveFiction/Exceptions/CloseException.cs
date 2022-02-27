namespace Heretic.InteractiveFiction.Exceptions;

public class CloseException: Exception
{
    public CloseException()
    {
    }

    public CloseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public CloseException(string message) : base(message)
    {
    }

    public CloseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}