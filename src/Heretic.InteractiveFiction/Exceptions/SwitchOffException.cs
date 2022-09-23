namespace Heretic.InteractiveFiction.Exceptions;

public class SwitchOffException: Exception
{
    public SwitchOffException()
    {
    }

    public SwitchOffException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public SwitchOffException(string message) : base(message)
    {
    }

    public SwitchOffException(string message, Exception innerException) : base(message, innerException)
    {
    } 
}