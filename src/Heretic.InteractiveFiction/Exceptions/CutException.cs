namespace Heretic.InteractiveFiction.Exceptions;

public class CutException: Exception
{
    public CutException()
    {
    }

    public CutException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public CutException(string message) : base(message)
    {
    }

    public CutException(string message, Exception innerException) : base(message, innerException)
    {
    }
}