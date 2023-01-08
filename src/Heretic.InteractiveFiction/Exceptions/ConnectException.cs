namespace Heretic.InteractiveFiction.Exceptions;

public class ConnectException: Exception
{
    public ConnectException()
    {
    }

    public ConnectException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ConnectException(string message) : base(message)
    {
    }

    public ConnectException(string message, Exception innerException) : base(message, innerException)
    {
    }   
}