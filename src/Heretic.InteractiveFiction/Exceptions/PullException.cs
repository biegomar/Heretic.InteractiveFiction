namespace Heretic.InteractiveFiction.Exceptions;

public class PullException: Exception
{
    public PullException()
    {
    }

    public PullException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public PullException(string message) : base(message)
    {
    }

    public PullException(string message, Exception innerException) : base(message, innerException)
    {
    }
}