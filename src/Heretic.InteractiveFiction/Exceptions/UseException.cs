namespace Heretic.InteractiveFiction.Exceptions;

public class UseException : Exception
{
    public UseException()
    {
    }

    public UseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public UseException(string message) : base(message)
    {
    }

    public UseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
