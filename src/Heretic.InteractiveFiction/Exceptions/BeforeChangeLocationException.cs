namespace Heretic.InteractiveFiction.Exceptions;

public class BeforeChangeLocationException : Exception
{
    public BeforeChangeLocationException()
    {
    }

    protected BeforeChangeLocationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public BeforeChangeLocationException(string message) : base(message)
    {
    }

    public BeforeChangeLocationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
