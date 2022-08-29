namespace Heretic.InteractiveFiction.Exceptions;

public class KindleException: Exception
{
    public KindleException()
    {
    }

    public KindleException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public KindleException(string message) : base(message)
    {
    }

    public KindleException(string message, Exception innerException) : base(message, innerException)
    {
    }
}