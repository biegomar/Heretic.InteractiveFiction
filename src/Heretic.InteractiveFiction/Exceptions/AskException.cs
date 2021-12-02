namespace Heretic.InteractiveFiction.Exceptions;

public class AskException : Exception
{
    public AskException()
    {
    }

    public AskException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public AskException(string message) : base(message)
    {
    }

    public AskException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
