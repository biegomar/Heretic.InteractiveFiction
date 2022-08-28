namespace Heretic.InteractiveFiction.Exceptions;

public class EatException: Exception
{
    public EatException()
    {
    }

    public EatException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public EatException(string message) : base(message)
    {
    }

    public EatException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
}