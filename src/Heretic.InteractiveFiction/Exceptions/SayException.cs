namespace Heretic.InteractiveFiction.Exceptions;

public class SayException : Exception
{
    public SayException()
    {
    }

    public SayException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public SayException(string message) : base(message)
    {
    }

    public SayException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
