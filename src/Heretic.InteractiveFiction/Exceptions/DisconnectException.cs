namespace Heretic.InteractiveFiction.Exceptions;

public class DisconnectException: Exception
{
    public DisconnectException()
    {
    }

    public DisconnectException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public DisconnectException(string message) : base(message)
    {
    }

    public DisconnectException(string message, Exception innerException) : base(message, innerException)
    {
    }
}