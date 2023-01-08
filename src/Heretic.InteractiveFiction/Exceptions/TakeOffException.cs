namespace Heretic.InteractiveFiction.Exceptions;

public class TakeOffException: Exception
{
    public TakeOffException()
    {
    }

    public TakeOffException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public TakeOffException(string message) : base(message)
    {
    }

    public TakeOffException(string message, Exception innerException) : base(message, innerException)
    {
    }  
}