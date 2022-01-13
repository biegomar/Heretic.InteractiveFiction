namespace Heretic.InteractiveFiction.Exceptions;

public class OpenException: Exception
{
    public OpenException()
    {
    }

    public OpenException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public OpenException(string message) : base(message)
    {
    }

    public OpenException(string message, Exception innerException) : base(message, innerException)
    {
    }
}