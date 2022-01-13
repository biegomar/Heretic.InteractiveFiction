namespace Heretic.InteractiveFiction.Exceptions;

public class BreakException: Exception
{
    public BreakException()
    {
    }

    public BreakException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public BreakException(string message) : base(message)
    {
    }

    public BreakException(string message, Exception innerException) : base(message, innerException)
    {
    }
}