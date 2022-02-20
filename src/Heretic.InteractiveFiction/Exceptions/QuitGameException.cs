namespace Heretic.InteractiveFiction.Exceptions;

public class QuitGameException: Exception
{
    public QuitGameException()
    {
    }

    public QuitGameException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public QuitGameException(string message) : base(message)
    {
    }

    public QuitGameException(string message, Exception innerException) : base(message, innerException)
    {
    }
}