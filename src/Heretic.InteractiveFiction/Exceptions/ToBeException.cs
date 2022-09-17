namespace Heretic.InteractiveFiction.Exceptions;

public class ToBeException: Exception
{
    public ToBeException()
    {
    }

    public ToBeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ToBeException(string message) : base(message)
    {
    }

    public ToBeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}