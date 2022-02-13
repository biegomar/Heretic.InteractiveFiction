namespace Heretic.InteractiveFiction.Exceptions;

public class TakeException: Exception
{
    public TakeException()
    {
    }

    public TakeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public TakeException(string message) : base(message)
    {
    }

    public TakeException(string message, Exception innerException) : base(message, innerException)
    {
    }   
}