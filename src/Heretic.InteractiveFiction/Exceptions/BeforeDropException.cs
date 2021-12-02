namespace Heretic.InteractiveFiction.Exceptions;

public class BeforeDropException : Exception
{
    public BeforeDropException()
    {
    }

    public BeforeDropException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public BeforeDropException(string message) : base(message)
    {
    }

    public BeforeDropException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
