namespace Heretic.InteractiveFiction.Exceptions;

public class DrinkException: Exception
{
    public DrinkException()
    {
    }

    public DrinkException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public DrinkException(string message) : base(message)
    {
    }

    public DrinkException(string message, Exception innerException) : base(message, innerException)
    {
    }
}