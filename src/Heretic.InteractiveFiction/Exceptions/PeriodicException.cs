namespace Heretic.InteractiveFiction.Exceptions;

public class PeriodicException: Exception
{
    public PeriodicException()
    {
    }

    public PeriodicException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public PeriodicException(string message) : base(message)
    {
    }

    public PeriodicException(string message, Exception innerException) : base(message, innerException)
    {
    }
}