namespace Heretic.InteractiveFiction.Exceptions;

public class ReadException: Exception
{
    public ReadException()
    {
    }

    public ReadException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ReadException(string message) : base(message)
    {
    }

    public ReadException(string message, Exception innerException) : base(message, innerException)
    {
    }
}