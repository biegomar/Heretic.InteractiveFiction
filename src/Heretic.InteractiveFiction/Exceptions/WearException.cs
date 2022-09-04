namespace Heretic.InteractiveFiction.Exceptions;

public class WearException: Exception
{
    public WearException()
    {
    }

    public WearException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public WearException(string message) : base(message)
    {
    }

    public WearException(string message, Exception innerException) : base(message, innerException)
    {
    }
}