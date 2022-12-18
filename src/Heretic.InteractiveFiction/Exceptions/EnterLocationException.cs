namespace Heretic.InteractiveFiction.Exceptions;

public class EnterLocationException: Exception
{
    public EnterLocationException()
    {
    }

    public EnterLocationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public EnterLocationException(string message) : base(message)
    {
    }

    public EnterLocationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}