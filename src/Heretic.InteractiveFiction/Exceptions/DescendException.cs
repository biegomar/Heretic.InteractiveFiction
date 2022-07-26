namespace Heretic.InteractiveFiction.Exceptions;

public class DescendException: Exception
{
    public DescendException()
    {
    }

    public DescendException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public DescendException(string message) : base(message)
    {
    }

    public DescendException(string message, Exception innerException) : base(message, innerException)
    {
    }
}