namespace Heretic.InteractiveFiction.Exceptions;

public class NoVerbException: Exception
{
    public NoVerbException()
    {
    }

    public NoVerbException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public NoVerbException(string message) : base(message)
    {
    }

    public NoVerbException(string message, Exception innerException) : base(message, innerException)
    {
    } 
}