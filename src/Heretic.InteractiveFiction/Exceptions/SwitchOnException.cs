namespace Heretic.InteractiveFiction.Exceptions;

public class SwitchOnException: Exception
{
    public SwitchOnException()
    {
    }

    public SwitchOnException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public SwitchOnException(string message) : base(message)
    {
    }

    public SwitchOnException(string message, Exception innerException) : base(message, innerException)
    {
    } 
}