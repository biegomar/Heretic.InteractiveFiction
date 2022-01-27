namespace Heretic.InteractiveFiction.Exceptions;

public class TurnException: Exception
{
    public TurnException()
    {
    }

    public TurnException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public TurnException(string message) : base(message)
    {
    }

    public TurnException(string message, Exception innerException) : base(message, innerException)
    {
    }
}