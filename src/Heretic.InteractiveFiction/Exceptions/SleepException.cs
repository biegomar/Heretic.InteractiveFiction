namespace Heretic.InteractiveFiction.Exceptions;

public class SleepException: Exception
{
    public SleepException()
    {
    }

    public SleepException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public SleepException(string message) : base(message)
    {
    }

    public SleepException(string message, Exception innerException) : base(message, innerException)
    {
    } 
}