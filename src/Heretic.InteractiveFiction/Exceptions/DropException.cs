namespace Heretic.InteractiveFiction.Exceptions;

public class DropException : Exception
{
    public DropException()
    {
    }

    public DropException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public DropException(string message) : base(message)
    {
    }

    public DropException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
