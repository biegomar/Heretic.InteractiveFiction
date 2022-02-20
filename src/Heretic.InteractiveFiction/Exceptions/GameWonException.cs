namespace Heretic.InteractiveFiction.Exceptions;

public class GameWonException: Exception
{
    public GameWonException()
    {
    }

    public GameWonException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public GameWonException(string message) : base(message)
    {
    }

    public GameWonException(string message, Exception innerException) : base(message, innerException)
    {
    }
}