namespace Heretic.InteractiveFiction.Exceptions;

public class SmellException: Exception
{
    public SmellException()
    {
    }

    public SmellException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public SmellException(string message) : base(message)
    {
    }

    public SmellException(string message, Exception innerException) : base(message, innerException)
    {
    } 
}