namespace Heretic.InteractiveFiction.Exceptions;

public class LookException: Exception
{
    public LookException()
    {
    }

    public LookException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public LookException(string message) : base(message)
    {
    }

    public LookException(string message, Exception innerException) : base(message, innerException)
    {
    } 
}