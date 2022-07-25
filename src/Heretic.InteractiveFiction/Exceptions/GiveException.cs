namespace Heretic.InteractiveFiction.Exceptions;

public class GiveException: Exception
{
    public GiveException()
    {
    }

    public GiveException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public GiveException(string message) : base(message)
    {
    }

    public GiveException(string message, Exception innerException) : base(message, innerException)
    {
    }
}