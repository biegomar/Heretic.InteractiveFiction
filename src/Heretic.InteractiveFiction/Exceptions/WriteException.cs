namespace Heretic.InteractiveFiction.Exceptions;

public class WriteException: Exception
{
    public WriteException()
    {
    }

    public WriteException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public WriteException(string message) : base(message)
    {
    }

    public WriteException(string message, Exception innerException) : base(message, innerException)
    {
    }   
}