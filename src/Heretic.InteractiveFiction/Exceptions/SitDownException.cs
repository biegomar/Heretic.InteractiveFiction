namespace Heretic.InteractiveFiction.Exceptions;

public class SitDownException: Exception
{
    public SitDownException()
    {
    }

    public SitDownException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public SitDownException(string message) : base(message)
    {
    }

    public SitDownException(string message, Exception innerException) : base(message, innerException)
    {
    }
}