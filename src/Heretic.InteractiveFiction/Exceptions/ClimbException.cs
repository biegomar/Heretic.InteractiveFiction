namespace Heretic.InteractiveFiction.Exceptions;

public class ClimbException: Exception
{
    public ClimbException()
    {
    }

    public ClimbException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ClimbException(string message) : base(message)
    {
    }

    public ClimbException(string message, Exception innerException) : base(message, innerException)
    {
    }
}